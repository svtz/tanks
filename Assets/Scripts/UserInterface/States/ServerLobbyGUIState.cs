//using svtz.Tanks.Network;
//using UnityEngine;

//namespace svtz.Tanks.UserInterface.States
//{
//    internal sealed class ServerLobbyGUIState : LobbyGUIState
//    {
//        public ServerLobbyGUIState(GUISkin guiSkin, CustomNetworkDiscovery networkDiscovery, CustomNetworkManager networkManager) : base(guiSkin, networkDiscovery, networkManager)
//        {
//        }

//        public override GUIState Key
//        {
//            get { return GUIState.ServerLobby; }
//        }

//        public override GUIState OnEscapePressed()
//        {
//            NetworkDiscovery.CustomStop();
//            return GUIState.StartServer;
//        }
//    }
//}