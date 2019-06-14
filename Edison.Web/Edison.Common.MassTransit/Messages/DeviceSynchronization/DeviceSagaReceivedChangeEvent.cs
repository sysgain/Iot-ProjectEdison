﻿using Edison.Common.Messages.Interfaces;
using System;

namespace Edison.Common.Messages
{
    public class DeviceSagaReceivedChangeEvent : IEventSagaReceivedDeviceChange
    {
        public Guid DeviceId { get; set; }
        public string ChangeType { get; set; }
        public DateTime Date { get; set; }
        public string Data { get; set; }
    }
}
