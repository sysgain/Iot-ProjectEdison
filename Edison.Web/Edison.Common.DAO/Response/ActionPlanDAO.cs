﻿using Edison.Core.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Edison.Common.Interfaces;

namespace Edison.Common.DAO
{
    /// <summary>
    /// DAO - Contains information pertaining to an action plan template
    /// </summary>
    public class ActionPlanDAO : IEntityDAO
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [JsonConverter(typeof(EpochDateTimeConverter))]
        public DateTime CreationDate { get; set; }
        [JsonConverter(typeof(EpochDateTimeConverter))]
        public DateTime UpdateDate { get; set; }
        public bool IsActive { get; set; }
        public string Color { get; set; }
        public string Icon { get; set; }
        public double PrimaryRadius { get; set; }
        public double SecondaryRadius { get; set; }
        public bool AcceptSafeStatus { get; set; }
        public IEnumerable<ActionDAOObject> OpenActions { get; set; }
        public IEnumerable<ActionDAOObject> CloseActions { get; set; }
        public string ETag { get; set; }
        
    }
}
