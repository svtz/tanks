using System;
using svtz.Tanks.Network;
using UnityEngine;

namespace svtz.Tanks.UserInterface.States
{
    internal sealed class GameMenuGUIState : NetworkMenuGUIState
    {
        public GameMenuGUIState(GUISkin guiSkin, CustomNetworkDiscovery networkDiscovery) : base(guiSkin, networkDiscovery)
        {
        }

        public override GUIState Key
        {
            get { return GUIState.GameMenu; }
        }

        public override GUIState OnGUI()
        {
            var nextState = Key;
            GUI.Box(GamePanel(), "");
            GUILayout.BeginArea(GamePanel(), GetStyle("MenuArea"));
            GUILayout.BeginVertical();
            GUILayout.Label("ПанкоТанки");
            GUILayout.Label("Игровое меню");

            if (GUILayout.Button("Вернуться в игру"))
                nextState = OnEscapePressed();
            if (GUILayout.Button("Покинуть игру"))
            {
                NetworkDiscovery.CustomStop();
                nextState = GUIState.MainMenu;
            }
            GUILayout.EndVertical();
            GUILayout.EndArea();

            return nextState;
        }

        public override GUIState OnEscapePressed()
        {
            return GUIState.InGame;
        }
    }
}