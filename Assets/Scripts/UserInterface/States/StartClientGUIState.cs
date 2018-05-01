using svtz.Tanks.Network;
using UnityEngine;

namespace svtz.Tanks.UserInterface.States
{
    internal sealed class StartClientGUIState : NetworkMenuGUIState
    {
        private Vector2 _scrollPosition = new Vector2(0, 0);

        public StartClientGUIState(GUISkin guiSkin, CustomNetworkDiscovery networkDiscovery) : base(guiSkin, networkDiscovery)
        {
        }

        public override GUIState Key
        {
            get { return GUIState.StartClient; }
        }

        public override GUIState OnGUI()
        {
            var nextState = Key;

            GUI.Box(GamePanel(), "");
            GUILayout.BeginArea(GamePanel(), GetStyle("MenuArea"));
            GUILayout.BeginVertical();
            GUILayout.Label("Введите имя");
            NetworkDiscovery.playerName =
                GUILayout.TextField(NetworkDiscovery.playerName);
            GUILayout.Label("Выберите игру");
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);
            foreach (ServerData record in NetworkDiscovery.findedServers)
            {
                if (GUILayout.Button(record.ServerName + "(" + record.NetworkAddress + ":" + record.Port + ")"))
                {
                    NetworkDiscovery.CustomStartClient(record);
                    nextState = GUIState.ClientLobby;
                }
            }
            GUILayout.EndScrollView();


            if (GUILayout.Button("Вернуться назад"))
                nextState = OnEscapePressed();
            GUILayout.EndVertical();
            GUILayout.EndArea();

            return nextState;
        }

        public override GUIState OnEscapePressed()
        {
            NetworkDiscovery.CustomStopServerDiscovery();
            return GUIState.MainMenu;
        }
    }
}