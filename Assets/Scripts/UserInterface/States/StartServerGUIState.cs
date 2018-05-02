using svtz.Tanks.Network;
using UnityEngine;

namespace svtz.Tanks.UserInterface.States
{
    internal sealed class StartServerGUIState : NetworkMenuGUIState
    {
        public override GUIState Key
        {
            get { return GUIState.StartServer; }
        }

        public override GUIState OnGUI()
        {
            var newState = Key;

            CenterScreen(() =>
            {
                GUILayout.Label("НОВАЯ ИГРА: ХОСТ", GetStyle("MenuTitle"));

                GUILayout.Label("Имя хоста:");
                NetworkDiscovery.serverName =
                    GUILayout.TextField(NetworkDiscovery.serverName);
                GUILayout.Label("Порт:");
                string newPort = GUILayout.TextField(NetworkDiscovery.networkPort.ToString());
                int i_new_port = 0;
                if (int.TryParse(newPort, out i_new_port))
                    NetworkDiscovery.networkPort = i_new_port;
                GUILayout.Label("Имя игрока:");
                NetworkDiscovery.playerName =
                    GUILayout.TextField(NetworkDiscovery.playerName);
                if (GUILayout.Button("СОЗДАТЬ"))
                {
                    if (NetworkDiscovery.CustomStartServer())
                        newState = GUIState.ServerLobby;
                }
                if (GUILayout.Button("ОТМЕНА", GetStyle("ReturnButton")))
                    newState = OnEscapePressed();
            });
            return newState;
        }

        public override GUIState OnEscapePressed()
        {
            return GUIState.MainMenu;
        }

        public StartServerGUIState(GUISkin guiSkin, CustomNetworkDiscovery networkDiscovery) : base(guiSkin, networkDiscovery)
        {
        }
    }
}