using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;

namespace svtz.Tanks.Network
{
    internal sealed class CustomNetworkDiscovery : NetworkDiscovery {
        public string serverName;
        public string playerName;
        public int networkPort = 7777;
        private ServerData serverData ;
        private CustomNetworkManager manager;
        public List<ServerData> foundServers = null;


        [Inject]
        public void Construct(CustomNetworkManager networkManager)
        {
            manager = networkManager;
            playerName = System.Environment.UserName;
            serverName = "Игра " + System.Environment.MachineName;
        }

        public override void OnReceivedBroadcast(string fromAddress, string data)
        {
            ServerData newServerData = ScriptableObject.CreateInstance<ServerData>();
            newServerData.Load(data);
            OnDiscovery(newServerData);
        }

        private void OnDiscovery(ServerData newServerData)
        {
            for (int index = 0; index < foundServers.Count; ++index)
            {
                if (foundServers[index].NetworkAddress == newServerData.NetworkAddress &&
                    foundServers[index].Port == newServerData.Port)
                {
                    foundServers[index] = newServerData;
                    return;
                }
            }
            foundServers.Add(newServerData);
        }

        public bool CustomStartServer()
        {
            manager.StartHost();
            if (manager.isNetworkActive && Initialize())
            {
                serverData = ScriptableObject.CreateInstance<ServerData>();
                serverData.ServerName = serverName;
                serverData.NetworkAddress = UnityEngine.Network.player.ipAddress;
                serverData.Port = networkPort;
                broadcastData = serverData.ToString();
                return StartAsServer();
            }
            return false;
        }
        public bool CustomStartClient(ServerData serverData)
        {
            CustomStopServerDiscovery();
            manager.networkAddress = serverData.NetworkAddress;
            manager.networkPort = serverData.Port;
            manager.StartClient();
            return manager.isNetworkActive;
        }

        public bool CustomStartServerDiscovery()
        {
            if (Initialize())
            {
                foundServers = new List<ServerData>();
                return StartAsClient();
            }
            else
                return false;
        }

        public void CustomStopServerDiscovery()
        {
            StopBroadcast();
        }

        public void CustomStop()
        {
            if (NetworkServer.active)
            {
            if (isServer)
                StopBroadcast();
                manager.StopHost();
            }
            else
                manager.StopClient();
        }

    }
}
