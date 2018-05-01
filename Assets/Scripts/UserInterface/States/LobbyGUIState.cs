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

        protected Rect LobbyPanel()
        {
            var style = GetStyle("LobbyArea");
            return new Rect(
                style.padding.left,
                style.padding.top,
                style.fixedWidth,
                Screen.height - style.padding.top - style.padding.bottom);
        }
    }
}