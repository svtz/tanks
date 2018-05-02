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

            Center(() =>
            {
                GUILayout.BeginVertical(GetStyle("InGameBox"));
                GUILayout.Label("ПанкоТанки", GetStyle("MenuTitle"));

                if (GUILayout.Button("Вернуться в бой"))
                {
                    nextState = OnEscapePressed();
                }

                if (GUILayout.Button("Выйти в меню", GetStyle("ReturnButton")))
                {
                    NetworkDiscovery.CustomStop();
                    nextState = GUIState.MainMenu;
                }
                GUILayout.EndVertical();
            });

            return nextState;
        }

        public override GUIState OnEscapePressed()
        {
            return GUIState.InGame;
        }
    }
}