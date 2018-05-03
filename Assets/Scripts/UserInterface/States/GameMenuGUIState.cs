//using System;
//using svtz.Tanks.Network;
//using UnityEngine;

//namespace svtz.Tanks.UserInterface.States
//{
//    internal sealed class GameMenuGUIState : NetworkMenuGUIState
//    {
//        public GameMenuGUIState(GUISkin guiSkin, CustomNetworkDiscovery networkDiscovery) : base(guiSkin, networkDiscovery)
//        {
//        }

//        public override GUIState Key
//        {
//            get { return GUIState.GameMenu; }
//        }

//        public override GUIState OnGUI()
//        {
//            var nextState = Key;

//            CenterScreen(() =>
//            {
//                GUILayout.BeginVertical(GetStyle("InGameBox"));
//                MenuTitle("ПанкоТанки");

//                if (GUILayout.Button("ЗАКРЫТЬ"))
//                {
//                    nextState = OnEscapePressed();
//                }

//                if (ReturnButton("В МЕНЮ"))
//                {
//                    NetworkDiscovery.CustomStop();
//                    nextState = GUIState.MainMenu;
//                }
//                GUILayout.EndVertical();
//            });

//            return nextState;
//        }

//        public override GUIState OnEscapePressed()
//        {
//            return GUIState.InGame;
//        }
//    }
//}