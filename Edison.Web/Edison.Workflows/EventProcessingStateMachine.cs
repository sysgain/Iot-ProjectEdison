﻿using System;
using Microsoft.Extensions.Options;
using MassTransit;
using Automatonymous;
using Edison.Core.Common.Models;
using Edison.Common.Config;
using Edison.Common.Messages.Interfaces;
using Edison.Common.Messages;
using Edison.Workflows.Config;

namespace Edison.Workflows
{
    /// <summary>
    /// Saga to handle the lifecycle of an event cluster
    /// </summary>
    internal class EventProcessingStateMachine : MassTransitStateMachine<EventProcessingState>
    {
        //Warning keep concurrency limit to 1
        private readonly ServiceBusRabbitMQOptions _configBus;
        private readonly WorkflowConfigEventProcessor _configWorkflow;

        public EventProcessingStateMachine(IOptions<ServiceBusRabbitMQOptions> configBus, IOptions<WorkflowConfig> configWorkflow)
        {
            _configBus = configBus.Value;
            _configWorkflow = configWorkflow.Value.EventProcessingWorkflow;

            InstanceState(x => x.State);

            #region Initialization Events
            //Main events being added, only one will be processed at a time
            Event(() => EventReceived, x => x.CorrelateBy(context => context.SagaDeviceId, context => $"{context.Message.EventType}_{context.Message.DeviceId}").SelectId(context => Guid.NewGuid()));
            Event(() => EventCloseReceived, x => x.CorrelateBy(context => context.SagaDeviceId, context => $"{context.Message.EventType}_{context.Message.DeviceId}"));

            Event(() => EventClusterCreatedOrUpdated, x => x.CorrelateById(context => context.Message.EventCluster.EventClusterId));
            Event(() => EventClusterClosed, x => x.CorrelateById(context => context.Message.EventCluster.EventClusterId));

            Event(() => EventClusterCreateOrUpdateRequestedFault, x => x.CorrelateById(context => context.Message.Message.EventClusterId));

            //UI Update notification, can be ignored if saga doesn't exist anymore
            //Event(() => EventClusterUIUpdated, x => { x.CorrelateById(context => context.Message.CorrelationId); x.OnMissingInstance(m => m.Discard()); });

            Schedule(() => EventClusterLifespanElapsed, x => x.ExpirationId, x =>
            {
                x.Delay = TimeSpan.FromMinutes(_configWorkflow.EventClusterLifespan);
                x.Received = e => e.CorrelateById(context => context.Message.EventClusterId);
            });
            #endregion

            //The event is submitted from Azure Service Queue
            Initially(
                When(EventReceived)
                    .Then(context => context.Instance.EventType = context.Data.EventType)
                    .Then(context => context.Instance.DeviceId = context.Data.DeviceId)
                    .Then(context => context.Instance.LastEventReceived = context.Data.Date)
                    .Then(context => context.Instance.SagaDeviceId = $"{context.Data.EventType}_{context.Data.DeviceId}")
                    .ThenAsync(context => Console.Out.WriteLineAsync($"EventProcessing-{context.Instance.CorrelationId}: Saga Initiated."))
                    .ThenAsync(context => Console.Out.WriteLineAsync($"EventProcessing--{context.Instance.CorrelationId}: Event Received."))
                    .If(new StateMachineCondition<EventProcessingState, IEventSagaReceived>(bc => bc.Data.EventType != "message"), x => x
                        .Schedule(EventClusterLifespanElapsed, p => new EventClusterLifespanElapsed() { EventClusterId = p.Instance.CorrelationId })
                    )
                    .ThenAsync(context => context.Publish(new EventClusterCreateOrUpdateRequestedEvent()
                    {
                        EventClusterId = context.Instance.CorrelationId,
                        DeviceId = context.Data.DeviceId,
                        EventType = context.Data.EventType,
                        Date = context.Data.Date,
                        Data = context.Data.Data
                    }))
                    .TransitionTo(ListeningToEvents)
                    );

            //State while the Event Cluster is being created
            During(ListeningToEvents,
                //Any new event will be sent to the same saga
                When(EventReceived)
                    .Then(context => context.Instance.LastEventReceived = context.Data.Date)
                    .ThenAsync(context => Console.Out.WriteLineAsync($"EventProcessing--{context.Instance.CorrelationId}: Event Received."))
                    .If(new StateMachineCondition<EventProcessingState, IEventSagaReceived>(bc => bc.Data.EventType != "message"), x => x
                        .Schedule(EventClusterLifespanElapsed, p => new EventClusterLifespanElapsed() { EventClusterId = p.Instance.CorrelationId })
                    )
                    .ThenAsync(context => context.Publish(new EventClusterCreateOrUpdateRequestedEvent()
                    {
                        EventClusterId = context.Instance.CorrelationId,
                        DeviceId = context.Data.DeviceId,
                        EventType = context.Data.EventType,
                        Date = context.Data.Date,
                        Data = context.Data.Data
                    })),
                //Result of an event being created or updated
                When(EventClusterCreatedOrUpdated)
                    .ThenAsync(context => Console.Out.WriteLineAsync($"EventProcessing---{context.Instance.CorrelationId}: Event Cluster Created."))
                    .ThenAsync(context => Console.Out.WriteLineAsync($"EventProcessing---{context.Instance.CorrelationId}: Event Added in Cluster."))
                    //In case of a lot of events being sent at once, skip UI refresh on some events
                    .If(new StateMachineCondition<EventProcessingState, IEventClusterCreatedOrUpdated>(bc =>
                        
                        bc.Instance.LastEventReceived == bc.Data.LastEventDate || //If not new event before the reception, send update
                        bc.Data.EventCluster.EventCount == 1 || //If first event, send update
                        bc.Data.EventCluster.EventCount % 10 == 0) //If many events at the same time, send every 5 events
                        , x => x 
                        .ThenAsync(context => context.Publish(new EventUIUpdateRequestedEvent() //Update UI
                        {
                            CorrelationId = context.Instance.CorrelationId,
                            EventClusterUI = new EventClusterUIModel()
                            {
                                EventCluster = context.Data.EventCluster,
                                UpdateType = context.Data.EventCluster.EventCount > 1 ? EventClusterUpdateType.UpdateEventCluster.ToString() : EventClusterUpdateType.NewEventCluster.ToString()
                            }
                        }))
                        
                    )
                    //Check response tagging, only for the first generated event
                    .If(new StateMachineCondition<EventProcessingState, IEventClusterCreatedOrUpdated>(bc =>
                        bc.Data.EventCluster.EventCount == 1), 
                        x => x
                        .ThenAsync(context => context.Publish(new ResponseTagNewEventClusterRequestedEvent()
                        {
                            EventClusterId = context.Instance.CorrelationId,
                            EventClusterGeolocation = context.Data.EventCluster.Device.Geolocation
                        }))),

                When(EventCloseReceived)
                    .ThenAsync(context => Console.Out.WriteLineAsync($"EventProcessing--{context.Instance.CorrelationId}: Event Cluster closing requested."))
                    .ThenAsync(context => context.Publish(new EventClusterCloseRequestedEvent()
                    {
                        EventClusterId = context.Instance.CorrelationId,
                        ClosureDate = DateTime.UtcNow,
                        EndDate = DateTime.UtcNow.AddMinutes(_configWorkflow.EventClusterCooldown)
                    })),

                When(EventClusterClosed)
                    .ThenAsync(context => Console.Out.WriteLineAsync($"EventProcessing---{context.Instance.CorrelationId}: Saga Closed."))
                    .ThenAsync(context => context.Publish(new EventUIUpdateRequestedEvent()
                    {
                        CorrelationId = context.Instance.CorrelationId,
                        EventClusterUI = new EventClusterUIModel()
                        {
                            EventCluster = context.Data.EventCluster,
                            UpdateType = EventClusterUpdateType.CloseEventCluster.ToString()
                        }
                    }))
                    .Finalize()   
                );

            //Stateless
            DuringAny(
                //No new event after a while, request to close the cluster
                When(EventClusterLifespanElapsed.Received)
                 .ThenAsync(context => Console.Out.WriteLineAsync($"EventProcessing--{context.Instance.CorrelationId}: Event Cluster closing requested."))
                 .ThenAsync(context => context.Publish(new EventClusterCloseRequestedEvent()
                 {
                     EventClusterId = context.Data.EventClusterId,
                     ClosureDate = DateTime.UtcNow,
                     EndDate = DateTime.UtcNow.AddMinutes(_configWorkflow.EventClusterCooldown)
                 })),
                //Fault while creating/adding an event to cluster
                When(EventClusterCreateOrUpdateRequestedFault)
                .ThenAsync(context => Console.Out.WriteLineAsync($"EventProcessing---{context.Instance.CorrelationId}: !!FAULT!! Event Cluster: {context.Data.Message}"))
                //UI Update acknoledgement - Disabled for performance improvement
                //When(EventClusterUIUpdated)
                //    .ThenAsync(context => Console.Out.WriteLineAsync($"EventProcessing---{context.Instance.CorrelationId}: UI Updated."))
                );


            //Delete persisted saga after completion
            SetCompletedWhenFinalized();
        }

        public State ListeningToEvents { get; private set; }

        #region "Events"
        public Event<IEventClusterCreatedOrUpdated> EventClusterCreatedOrUpdated { get; private set; }
        public Event<Fault<IEventClusterCreateOrUpdateRequested>> EventClusterCreateOrUpdateRequestedFault { get; private set; }
        public Event<IEventClusterClosed> EventClusterClosed { get; private set; }

        public Event<IEventSagaReceived> EventReceived { get; private set; }
        public Event<IEventCloseSagaReceived> EventCloseReceived { get; private set; }
        //public Event<IEventUIUpdated> EventClusterUIUpdated { get; private set; }
        public Schedule<EventProcessingState, IEventClusterLifespanElapsed> EventClusterLifespanElapsed { get; private set; }
        #endregion
    }
}
