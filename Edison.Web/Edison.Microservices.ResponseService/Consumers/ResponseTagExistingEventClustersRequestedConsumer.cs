﻿using Edison.Common.Messages;
using Edison.Common.Messages.Interfaces;
using Edison.Core.Common.Models;
using Edison.Core.Interfaces;
using MassTransit;
using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Edison.ResponseService.Consumers
{
    public class ResponseTagExistingEventClustersRequestedConsumer : IConsumer<IResponseTagExistingEventClustersRequested>
    {
        private readonly IEventClusterRestService _eventClusterRestService;
        private readonly IResponseRestService _responseRestService;
        private readonly ILogger<ResponseTagExistingEventClustersRequestedConsumer> _logger;

        public ResponseTagExistingEventClustersRequestedConsumer(IEventClusterRestService eventClusterRestService,
            IResponseRestService responseRestService,
            ILogger<ResponseTagExistingEventClustersRequestedConsumer> logger)
        {
            _logger = logger;
            _eventClusterRestService = eventClusterRestService;
            _responseRestService = responseRestService;
        }

        public async Task Consume(ConsumeContext<IResponseTagExistingEventClustersRequested> context)
        {
            try
            {
                _logger.LogDebug($"EventClusterAssignResponseRequestedConsumer: Retrieved message from response '{context.Message.ResponseId}'.");

                var eventClusterIds = await _eventClusterRestService.GetClustersInRadius(new EventClusterGeolocationModel()
                {
                    Radius = context.Message.Radius,
                    ResponseEpicenterLocation = context.Message.ResponseGeolocation
                });
                if (eventClusterIds != null)
                {
                    if(eventClusterIds.ToList().Count == 0)
                    {
                        _logger.LogDebug($"EventClusterAssignResponseRequestedConsumer: No Event Clusters found.");
                        return;
                    }

                    _logger.LogDebug($"EventClusterAssignResponseRequestedConsumer: Event Cluster Ids retrieved.");

                    //Add event cluster Ids to response
                    var result = await _responseRestService.AddEventClusterIdsToResponse(new ResponseEventClustersUpdateModel()
                    {
                        ResponseId = context.Message.ResponseId,
                        EventClusterIds = eventClusterIds
                    });
                    if (result != null)
                    {
                        //Push message to Service bus queue for UI update
                        _logger.LogDebug($"EventClusterAssignResponseRequestedConsumer: Event Cluster Ids added to response '{context.Message.ResponseId}'.");
                        await context.Publish(new ResponseTaggedEventClusterEvents() { Response = result });
                        return;
                    }
                    _logger.LogError($"EventClusterAssignResponseRequestedConsumer: Event Cluster Ids could not be added to response '{context.Message.ResponseId}'.");
                    throw new Exception($"Event Cluster Ids could not be added to response '{context.Message.ResponseId}'.");
                }
                _logger.LogError("EventClusterAssignResponseRequestedConsumer: Event Cluster Ids could not be retrieved.");
                throw new Exception("Event Cluster Ids could not be retrieved.");

            }
            catch(Exception e)
            {
                _logger.LogError($"EventClusterAssignResponseRequestedConsumer: {e.Message}");
                throw e;
            }
        }
    }
}
