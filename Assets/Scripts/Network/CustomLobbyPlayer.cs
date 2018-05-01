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

        public void DrawGUI(GUISkin skin)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(_playerName, skin.GetStyle("LobbyPlayerLabel"));
            if (isLocalPlayer)
            {
                if (GUILayout.Button(readyToBegin ? "ГОТОВ" : "Не готов", skin.GetStyle("ReadyButton")))
                {
                    readyToBegin = !readyToBegin;
                    if (readyToBegin)
                        SendReadyToBeginMessage();
                    else
                        SendNotReadyToBeginMessage();
                }
            }else
            {
                GUILayout.Label(readyToBegin ? "ГОТОВ" : "Не готов", skin.GetStyle("ReadyButton"));
            }
            GUILayout.EndHorizontal();
        }
    }
}
