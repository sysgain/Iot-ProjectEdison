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
    public class ResponseTagNewEventClusterRequestedConsumer : IConsumer<IResponseTagNewEventClusterRequested>
    {
        private readonly IEventClusterRestService _eventClusterRestService;
        private readonly IResponseRestService _responseRestService;
        private readonly ILogger<ResponseTagNewEventClusterRequestedConsumer> _logger;

        public ResponseTagNewEventClusterRequestedConsumer(IEventClusterRestService eventClusterRestService,
            IResponseRestService responseRestService,
            ILogger<ResponseTagNewEventClusterRequestedConsumer> logger)
        {
            _logger = logger;
            _eventClusterRestService = eventClusterRestService;
            _responseRestService = responseRestService;
        }

        public async Task Consume(ConsumeContext<IResponseTagNewEventClusterRequested> context)
        {
            try
            {
                _logger.LogDebug($"ResponseTagNewEventClusterRequestedConsumer: Retrieved message from event cluster '{context.Message.EventClusterId}'.");

                //Check if responses are associated
                IEnumerable<ResponseModel> responsesAssociated = await _responseRestService.GetResponsesFromPointRadius(new ResponseGeolocationModel()
                {
                    EventClusterEpicenterLocation = context.Message.EventClusterGeolocation
                });
                if (responsesAssociated != null)
                {
                    //Add to every matching response
                    foreach (var responseAssociated in responsesAssociated)
                    {
                        //Add to matching response
                        ResponseModel result = await _responseRestService.AddEventClusterIdsToResponse(new ResponseEventClustersUpdateModel()
                        {
                            ResponseId = responseAssociated.ResponseId,
                            EventClusterIds = new List<Guid>() { context.Message.EventClusterId }
                        });
                        if (result != null)
                        {
                            //Publish update for saga
                            _logger.LogDebug($"ResponseTagNewEventClusterRequestedConsumer: Event Cluster Id added to response '{result.ResponseId}'.");
                            await context.Publish(new ResponseTaggedEventClusterEvents() { Response = result });
                            return;
                        }
                        _logger.LogError($"ResponseTagNewEventClusterRequestedConsumer: Event Cluster Id could not be added to response '{result.ResponseId}'.");
                        throw new Exception($"Event Cluster Id could not be added to response '{result.ResponseId}'.");
                    }
                    return;
                }
            }
            catch(Exception e)
            {
                _logger.LogError($"ResponseTagNewEventClusterRequestedConsumer: {e.Message}");
                throw e;
            }
        }
    }
}
