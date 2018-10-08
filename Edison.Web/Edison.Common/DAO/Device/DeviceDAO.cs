﻿using Edison.Core.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Edison.Common.DAO
{
    public class DeviceDAO : IEntityDAO
    {
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }
        public string DeviceType { get; set; }
        [JsonConverter(typeof(EpochDateTimeConverter))]
        public DateTime LastAccessTime { get; set; }
        public bool Sensor { get; set; }
        public string LocationName { get; set; }
        public string LocationLevel1 { get; set; }
        public string LocationLevel2 { get; set; }
        public string LocationLevel3 { get; set; }
        public GeolocationDAOObject Geolocation { get; set; }
        public DateTime CreationDate { get; set; }       
        public DateTime UpdateDate { get; set; }
        public Dictionary<string, object> Custom { get; set; }
        public Dictionary<string, object> Reported { get; set; }
        public Dictionary<string, object> Desired { get; set; }
        public string ETag { get; set; }
        
    }
}
