//using svtz.Tanks.Network;
//using UnityEngine;

//namespace svtz.Tanks.UserInterface.States
//{
//    internal sealed class ClientLobbyGUIState : LobbyGUIState
//    {
//        public ClientLobbyGUIState(GUISkin guiSkin, CustomNetworkDiscovery networkDiscovery, CustomNetworkManager networkManager) : base(guiSkin, networkDiscovery, networkManager)
//        {
//        }

//        public override GUIState Key
//        {
//            get { return GUIState.ClientLobby; }
//        }

//        public override GUIState OnEscapePressed()
//        {
//            NetworkDiscovery.CustomStop();
//            NetworkDiscovery.CustomStartServerDiscovery();
//            return GUIState.StartClient;
//        }
//    }
//}