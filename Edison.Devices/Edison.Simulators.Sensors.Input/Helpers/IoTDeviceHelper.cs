﻿using Edison.Simulators.Sensors.Input.Config;
using Edison.Simulators.Sensors.Input.Models;
using Edison.Simulators.Sensors.Input.Models.Helpers;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Edison.Simulators.Sensors.Input.Helpers
{
    public class IoTDeviceHelper
    {
        private readonly RegistryManager _registryManager;
        private readonly JobClient _jobClient;
        private readonly SimulatorConfig _config;
        private List<IoTDevice> _CacheDevices = null;

        public IoTDeviceHelper(IOptions<SimulatorConfig> config)
        {
            _config = config.Value;
            _registryManager = RegistryManager.CreateFromConnectionString(_config.IoTHubConnectionString);
            _jobClient = JobClient.CreateFromConnectionString(_config.IoTHubConnectionString);
        }

        public async Task CreateDevice(IoTDevice newDevice, bool overrideTags)
        {
            //reset cache
            EmptyCacheDevices();

            Device device = await _registryManager.GetDeviceAsync(newDevice.DeviceId);
            if (device == null)
            {
                ConsoleHelper.WriteInfo($"Create new demo device '{newDevice.DeviceId}'");
                device = await _registryManager.AddDeviceAsync(new Device(newDevice.DeviceId));
            }
            else
            {
                if(overrideTags)
                    ConsoleHelper.WriteInfo($"The device '{newDevice.DeviceId}' already exist. Updating tags.");
                else
                {
                    ConsoleHelper.WriteInfo($"The device '{newDevice.DeviceId}' already exist. Skipping.");
                    return;
                }
            }

            Twin twin = new Twin
            {
                Tags = new TwinCollection()
            };
            if (newDevice.Demo)
                twin.Tags["Demo"] = newDevice.Demo;
            twin.Tags["Sensor"] = newDevice.Sensor;
            twin.Tags["DeviceType"] = newDevice.DeviceType;
            twin.Tags["LocationName"] = newDevice.LocationName;
            twin.Tags["LocationLevel1"] = newDevice.LocationLevel1;
            twin.Tags["LocationLevel2"] = newDevice.LocationLevel2;
            twin.Tags["LocationLevel3"] = newDevice.LocationLevel3;
            twin.Tags["Geolocation"] = new Geolocation()
            {
                Latitude = newDevice.Latitude,
                Longitude = newDevice.Longitude
            };
            twin.ETag = "*";

            await _registryManager.UpdateTwinAsync(newDevice.DeviceId, twin, twin.ETag);
        }

        private async Task<List<IoTDevice>> GetDevices()
        {
            if (_CacheDevices != null)
                return _CacheDevices;

            List<IoTDevice> iotDevices = new List<IoTDevice>();
            IQuery query = _registryManager.CreateQuery(
                "SELECT deviceId, " +
                "tags.Demo as Demo, " +
                "tags.Sensor as Sensor, " +
                "tags.DeviceType as DeviceType, " +
                "tags.LocationName as LocationName, " +
                "tags.LocationLevel1 as LocationLevel1, " +
                "tags.LocationLevel2 as LocationLevel2, " +
                "tags.LocationLevel3 as LocationLevel3, " +
                "tags.Geolocation.Latitude as Latitude, " +
                "tags.Geolocation.Longitude as Longitude " +
                "FROM devices");
            while (query.HasMoreResults)
            {
                var page = await query.GetNextAsJsonAsync();
                foreach (var result in page)
                {
                    var device = JsonConvert.DeserializeObject<IoTDevice>(result);
                    iotDevices.Add(device);
                }
            }
            _CacheDevices = iotDevices;
            return iotDevices;
        }

        public async Task<List<IoTDevice>> GetDemoDevices()
        {
            List<IoTDevice> devices = await GetDevices();
            return devices.Where(p => p.Demo).ToList();
        }

        public async Task<List<IoTDevice>> GetAllInputDevices()
        {
            List<IoTDevice> devices = await GetDevices();
            return devices.Where(p => p.Sensor).ToList();
        }

        public async Task DeleteMultipleDevicesAsync(List<IoTDevice> iotDevices)
        {
            try
            {
                //reset cache
                EmptyCacheDevices();

                ConsoleHelper.WriteInfo($"Deleting {iotDevices.Count} demo devices...");
                List<Device> devices = new List<Device>();
                foreach (var iotDevice in iotDevices)
                {
                    Device device = new Device(iotDevice.DeviceId);
                    if (device != null)
                    {
                        devices.Add(device);
                    }
                }

                if (devices.Count > 0)
                    await _registryManager.RemoveDevices2Async(devices, true, new System.Threading.CancellationToken());
            }
            catch (Exception e)
            {
                ConsoleHelper.WriteError($"Delete multiple devices error {e.Message}");
                throw e;
            }
        }

        public async Task SendMessage(IoTDevice device, string eventType, DeviceTriggerIoTMessage message)
        {
            if (device.Client == null)
                device.Client = DeviceClient.CreateFromConnectionString(_config.IoTHubConnectionString, device.DeviceId);

            string messageJson = JsonConvert.SerializeObject(message);

            var messageIoT = new Microsoft.Azure.Devices.Client.Message(Encoding.UTF8.GetBytes(messageJson));
            messageIoT.Properties.Add("opType", "eventDevice");
            messageIoT.Properties.Add("eventType", eventType);

            await device.Client.SendEventAsync(messageIoT);
        }

        private void EmptyCacheDevices()
        {
            if (_CacheDevices == null)
                return;

            foreach(var device in _CacheDevices)
            {
                if (device.Client != null)
                    device.Client.Dispose();
            }
            _CacheDevices = null;
        }
    }
}
