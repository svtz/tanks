using svtz.Tanks.Network;
using UnityEngine;

namespace svtz.Tanks.UserInterface.States
{
    internal abstract class LobbyGUIState : NetworkMenuGUIState
    {
        protected CustomNetworkManager NetworkManager { get; private set; }

        protected LobbyGUIState(GUISkin guiSkin, CustomNetworkDiscovery networkDiscovery,
            CustomNetworkManager networkManager) : base(guiSkin, networkDiscovery)
        {
            NetworkManager = networkManager;
        }

        public sealed override GUIState OnGUI()
        {
            var nextState = Key;

            Center(() =>
            {
                foreach (var player in NetworkManager.lobbySlots)
                {
                    var customLobbyPlayer = player as CustomLobbyPlayer;
                    if (customLobbyPlayer != null)
                    {
                        customLobbyPlayer.DrawGUI(GuiSkin);
                    }
                }

                if (GUILayout.Button("Вернуться назад", GetStyle("ReturnButton")))
                {
                    nextState = OnEscapePressed();
                }
            });

            return nextState;
        }
    }
}