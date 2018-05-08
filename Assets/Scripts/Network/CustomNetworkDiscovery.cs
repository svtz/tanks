using System;
using System.Collections.Generic;
using UnityEngine.Networking;
using Zenject;

namespace svtz.Tanks.Network
{
    internal sealed class CustomNetworkDiscovery : NetworkDiscovery {
        public string ServerName;
        public string PlayerName;
        public int NetworkPort = 7777;
        private ServerData _serverData ;
        private CustomNetworkManager _netManager;
        public List<ServerData> FoundServers;

        private Action<ServerData> _onServerAvailable;

        [Inject]
        public void Construct(CustomNetworkManager networkManager)
        {
            _netManager = networkManager;
            PlayerName = Environment.UserName;
            ServerName = "Игра " + Environment.MachineName;
        }

        public override void OnReceivedBroadcast(string fromAddress, string data)
        {
            var newServerData = ServerData.Parse(data);
            OnDiscovery(newServerData);
        }

        private void OnDiscovery(ServerData newServerData)
        {
            for (var index = 0; index < FoundServers.Count; ++index)
            {
                var current = FoundServers[index];
                if (current != null && current.SameServerAs(newServerData))
                {
                    FoundServers[index] = newServerData;
                    _onServerAvailable(newServerData);
                    return;
                }
            }
            FoundServers.Add(newServerData);
            _onServerAvailable(newServerData);
        }

        public bool CustomStartServer()
        {
            _netManager.networkPort = NetworkPort;
            _netManager.StartHost();
            if (_netManager.isNetworkActive && Initialize())
            {
                _serverData = ServerData.Create(ServerName, UnityEngine.Network.player.ipAddress, NetworkPort);
                broadcastData = _serverData.ToString();
                return StartAsServer();
            }
            return false;
        }

        public bool CustomStartClient(ServerData serverData)
        {
            CustomStopServerDiscovery();
            _netManager.networkAddress = serverData.NetworkAddress;
            _netManager.networkPort = serverData.Port;
            _netManager.StartClient();
            return _netManager.isNetworkActive;
        }

        public bool CustomStartServerDiscovery(Action<ServerData> serverAvailable)
        {
            if (Initialize())
            {
                FoundServers = new List<ServerData>();
                _onServerAvailable = serverAvailable;
                return StartAsClient();
            }
            else
                return false;
        }

        public void CustomStopServerDiscovery()
        {
            StopBroadcast();
            _onServerAvailable = null;
        }

        public void CustomStop()
        {
            if (NetworkServer.active)
            {
                if (isServer)
                    StopBroadcast();
                _netManager.StopHost();
            }
            else
                _netManager.StopClient();
        }

    }
}
