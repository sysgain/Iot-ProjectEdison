﻿using System;

namespace Edison.Core.Common.Models
{
    public class ResponseCreationModel
    {
        public Guid ResponderUserId { get; set; }
        public ResponseActionPlanModel ActionPlan { get; set; }
        public Guid? PrimaryEventClusterId { get; set; }
        public Geolocation Geolocation { get; set; }
        public bool DelayStart { get; set; }
    }
}
