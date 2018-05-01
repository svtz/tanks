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
        public List<ServerData> findedServers = null;


        [Inject]
        public void Construct(CustomNetworkManager networkManager)
        {
            manager = networkManager;
        }

        void Start()
        {
            playerName = System.Environment.MachineName;
            serverName = System.Environment.MachineName;
        }
    

        public override void OnReceivedBroadcast(string fromAddress, string data)
        {
            ServerData newServerData = ScriptableObject.CreateInstance<ServerData>();
            newServerData.Load(data);
            OnDiscovery(newServerData);
            Debug.Log(data);
        }

        private void OnDiscovery(ServerData newServerData)
        {
            for (int index = 0; index < findedServers.Count; ++index)
            {
                if (findedServers[index].NetworkAddress == newServerData.NetworkAddress &&
                    findedServers[index].Port == newServerData.Port)
                {
                    findedServers[index] = newServerData;
                    return;
                }
            }
            findedServers.Add(newServerData);
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
                findedServers = new List<ServerData>();
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
            if (isServer)
            {
                StopBroadcast();
                manager.StopHost();
            }
            else
                manager.StopClient();
        }

    }
}
