using System;
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

            CenterScreen(() =>
            {
                MenuTitle("ПанкоТанки");

                if (GUILayout.Button("НОВАЯ ИГРА: ХОСТ"))
                {
                    nextState = GUIState.StartServer;
                }

                if (GUILayout.Button("НОВАЯ ИГРА: ПОИСК"))
                {
                    NetworkDiscovery.CustomStartServerDiscovery();
                    nextState = GUIState.StartClient;
                }

                if (ReturnButton("ВЫХОД"))
                {
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
#else
                    Application.Quit();
#endif
                }
            });

            return nextState;
        }

        public override GUIState OnEscapePressed()
        {
            return Key;
        }
    }
}