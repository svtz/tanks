using UnityEngine;
using UnityEngine.Networking;
using Zenject;

namespace svtz.Tanks.Network
{
    internal sealed class CustomLobbyPlayer : NetworkLobbyPlayer {
        [SyncVar]
        private string _playerName;

        public string PlayerName { get { return _playerName; } }

        private CustomNetworkDiscovery _networkDiscovery;

        [Inject]
        public void Construct(CustomNetworkDiscovery networkDiscovery)
        {
            _networkDiscovery = networkDiscovery;
        }

        public void ToggleReady()
        {
            readyToBegin = !readyToBegin;
            if (readyToBegin)
                SendReadyToBeginMessage();
            else
                SendNotReadyToBeginMessage();
        }

        // Use this for initialization
        void Start () {
            if (isLocalPlayer)
            {
                CmdSetName(_networkDiscovery.playerName);
            }
        }

        [Command]
        public void CmdSetName(string name)
        {
            _playerName = name;
        }
    }
}
