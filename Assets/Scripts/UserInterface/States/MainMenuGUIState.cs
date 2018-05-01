using svtz.Tanks.Network;
using UnityEngine;

namespace svtz.Tanks.UserInterface.States
{
    internal sealed class MainMenuGUIState : NetworkMenuGUIState
    {
        public MainMenuGUIState(GUISkin guiSkin, CustomNetworkDiscovery networkDiscovery) : base(guiSkin, networkDiscovery)
        {
        }

        public override GUIState Key
        {
            get { return GUIState.MainMenu; }
        }

        public override GUIState OnGUI()
        {
            var nextState = Key;

            GUI.Box(GamePanel(), "");
            GUILayout.BeginArea(GamePanel(), GetStyle("MenuArea"));
            GUILayout.BeginVertical();
            GUILayout.Label("ПанкоТанки");
            if (GUILayout.Button("Присоединиться к игре"))
            {
                NetworkDiscovery.CustomStartServerDiscovery();
                nextState = GUIState.StartClient;
            }
            if (GUILayout.Button("Создать игру"))
                nextState = GUIState.StartServer;
            if (GUILayout.Button("Покинуть игру"))
                Application.Quit();
            GUILayout.EndVertical();
            GUILayout.EndArea();

            return nextState;
        }

        public override GUIState OnEscapePressed()
        {
            return Key;
        }
    }
}