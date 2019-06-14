﻿using Edison.Devices.Onboarding.Common.Models;
using Edison.Devices.Onboarding.Helpers;
using Edison.Devices.Onboarding.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.WiFi;

namespace Edison.Devices.Onboarding.Services
{
    internal class WifiService
    {
        public WifiService()
        {

        }

        public async Task<ResultCommandNetworkStatus> ConnectToNetworkHandler(RequestCommandConnectToNetwork requestConnectToNetwork)
        {
            try
            {
                NetworkInformation networkInfo = requestConnectToNetwork.NetworkInformation;

                var wifiSet = await FindWifi(networkInfo.Ssid);
                if (wifiSet != null)
                {
                    if (!await PortalApiHelper.DisconnectFromNetwork(wifiSet.Adapter.NetworkAdapter.NetworkAdapterId))
                        DebugHelper.LogWarning($"Error while trying to disconnect from network: {wifiSet.Adapter.NetworkAdapter.NetworkAdapterId}");

                    var availableNetworks = await PortalApiHelper.GetAvailableNetworks(wifiSet.Adapter.NetworkAdapter.NetworkAdapterId);
                    foreach(var network in availableNetworks.AvailableNetworks)
                    {
                        if (network.ProfileAvailable && !await PortalApiHelper.DeleteNetworkProfile(wifiSet.Adapter.NetworkAdapter.NetworkAdapterId, network.ProfileName))
                            DebugHelper.LogWarning($"Error while trying to disconnect from network: {wifiSet.Adapter.NetworkAdapter.NetworkAdapterId}");
                    }
                    if (!await PortalApiHelper.ConnectToNetwork(wifiSet.Adapter.NetworkAdapter.NetworkAdapterId, networkInfo.Ssid, networkInfo.Password))
                        return ResultCommand.CreateFailedCommand<ResultCommandNetworkStatus>($"Error ConnectToNetworkHandler: Could not connect to network '{networkInfo.Ssid}'.");

                    return new ResultCommandNetworkStatus() { IsSuccess = true, Status = "Connected" };

                }
                return ResultCommand.CreateFailedCommand<ResultCommandNetworkStatus>($"Error ConnectToNetworkHandler: UnspecifiedFailure.");
            }
            catch(Exception e)
            {
                DebugHelper.LogError($"Error ConnectToNetworkHandler: {e.Message}.");
                return ResultCommand.CreateFailedCommand<ResultCommandNetworkStatus>($"Error ConnectToNetworkHandler: {e.Message}.");
            }
        }

        public async Task<ResultCommandNetworkStatus> DisconnectFromNetworkHandler(RequestCommandDisconnectFromNetwork requestDisconnectFromNetwork)
        {
            try
            {
                var wifiSet = await FindWifi(requestDisconnectFromNetwork.Ssid);
                if (wifiSet != null)
                {
                    DebugHelper.LogInformation($"Disconnecting from network: {wifiSet.Network.Ssid}");
                    wifiSet.Adapter.Disconnect();
                    return new ResultCommandNetworkStatus() { Status = "Disconnected", IsSuccess = true };
                }
                return ResultCommand.CreateFailedCommand<ResultCommandNetworkStatus>($"Error DisconnectFromNetworkHandler: UnspecifiedFailure.");
            }
            catch (Exception e)
            {
                DebugHelper.LogError($"Error DisconnectFromNetworkHandler: {e.Message}.");
                return ResultCommand.CreateFailedCommand<ResultCommandNetworkStatus>($"Error DisconnectFromNetworkHandler: {e.Message}.");
            }
        }

        public async Task<ResultCommandAvailableNetworks> GetAvailableNetworkListHandler()
        {
            try
            {                
                var wifiAdapterList = await WiFiAdapter.FindAllAdaptersAsync();

                List<AvailableNetwork> networks = new List<AvailableNetwork>();

                foreach (var adapter in wifiAdapterList)
                {
                    try
                    {
                        var availableNetworks = await PortalApiHelper.GetAvailableNetworks(adapter.NetworkAdapter.NetworkAdapterId);

                        if (availableNetworks != null && availableNetworks.AvailableNetworks != null)
                        {
                            networks.AddRange(availableNetworks.AvailableNetworks);
                        }
                    }
                    catch (Exception) { }
                }
                
                var sortedNetworks = networks.OrderBy(x => x.SSID).Distinct().ToList();

                return new ResultCommandAvailableNetworks() { Networks = sortedNetworks, IsSuccess = true };
            }
            catch (Exception e)
            {
                DebugHelper.LogError($"Error GetAvailableNetworkListHandler: {e.Message}.");
                return ResultCommand.CreateFailedCommand<ResultCommandAvailableNetworks>($"Error GetAvailableNetworkListHandler: {e.Message}.");
            }
        }

        public async Task<WifiSet> FindWifi(string ssid)
        {
            var wifiAdapterList = await WiFiAdapter.FindAllAdaptersAsync();
            var wifiList = from adapter in wifiAdapterList
                           from network in adapter.NetworkReport.AvailableNetworks
                           where network.Ssid.Equals(ssid)
                           select new WifiSet() { Adapter = adapter, Network = network };
            return wifiList.First();
        }
    }
}
