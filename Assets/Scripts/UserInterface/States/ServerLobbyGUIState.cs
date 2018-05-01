using svtz.Tanks.Network;
using UnityEngine;
using UnityEngine.Networking;

namespace svtz.Tanks.UserInterface.States
{
    internal sealed class ServerLobbyGUIState : LobbyGUIState
    {
        public ServerLobbyGUIState(GUISkin guiSkin, CustomNetworkDiscovery networkDiscovery, CustomNetworkManager networkManager) : base(guiSkin, networkDiscovery, networkManager)
        {
        }

        public override GUIState Key
        {
            get { return GUIState.ServerLobby; }
        }

        public override GUIState OnGUI()
        {
            var nextState = Key;

            GUI.Box(LobbyPanel(), "");
            GUILayout.BeginArea(LobbyPanel(), GetStyle("LobbyArea"));
            GUILayout.BeginVertical();
            foreach (NetworkLobbyPlayer player in NetworkManager.lobbySlots)
            {
                if (player != null)
                    ((CustomLobbyPlayer)player).DrawGUI();
            }
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Вернуться назад"))
                nextState = OnEscapePressed();

            GUILayout.EndVertical();
            GUILayout.EndArea();

            return nextState;
        }

        public override GUIState OnEscapePressed()
        {
            NetworkDiscovery.CustomStop();
            return GUIState.StartServer;
        }
    }
}