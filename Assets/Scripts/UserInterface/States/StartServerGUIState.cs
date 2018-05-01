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

            GUI.Box(GamePanel(), "");
            GUILayout.BeginArea(GamePanel(), GetStyle("MenuArea"));
            GUILayout.BeginVertical();
            GUILayout.Label("Введите название игры");
            NetworkDiscovery.serverName =
                GUILayout.TextField(NetworkDiscovery.serverName);
            GUILayout.Label("Введите порт");
            string newPort = GUILayout.TextField(NetworkDiscovery.networkPort.ToString());
            int i_new_port = 0;
            if (int.TryParse(newPort, out i_new_port))
                NetworkDiscovery.networkPort = i_new_port;
            GUILayout.Label("Введите имя");
            NetworkDiscovery.playerName =
                GUILayout.TextField(NetworkDiscovery.playerName);
            if (GUILayout.Button("Создать игру"))
            {
                if (NetworkDiscovery.CustomStartServer())
                    newState = GUIState.ServerLobby;
            }
            if (GUILayout.Button("Вернуться назад"))
                newState = OnEscapePressed();
            GUILayout.EndVertical();
            GUILayout.EndArea();

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