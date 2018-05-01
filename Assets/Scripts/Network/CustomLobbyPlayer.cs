using UnityEngine;
using UnityEngine.Networking;
using Zenject;

namespace svtz.Tanks.Network
{
    internal sealed class CustomLobbyPlayer : NetworkLobbyPlayer {
        [SyncVar]
        private string _playerName;

        private CustomNetworkDiscovery _networkDiscovery;

        [Inject]
        public void Construct(CustomNetworkDiscovery networkDiscovery)
        {
            _networkDiscovery = networkDiscovery;
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

        public void DrawGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(_playerName, GUILayout.Width(100));
            if (isLocalPlayer)
            {
                if (GUILayout.Button(readyToBegin ? "ГОТОВ" : "Не готов"))
                {
                    readyToBegin = !readyToBegin;
                    if (readyToBegin)
                        SendReadyToBeginMessage();
                    else
                        SendNotReadyToBeginMessage();
                }
            }else
            {
                GUILayout.Label(readyToBegin ? "ГОТОВ" : "Не готов");
            }
            GUILayout.EndHorizontal();
        }
    }
}
