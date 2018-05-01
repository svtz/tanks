using svtz.Tanks.Network;
using UnityEngine;

namespace svtz.Tanks.UserInterface.States
{
    internal abstract class NetworkMenuGUIState : AbstractGUIState
    {
        protected CustomNetworkDiscovery NetworkDiscovery { get; private set; }

        protected NetworkMenuGUIState(GUISkin guiSkin, CustomNetworkDiscovery networkDiscovery) : base(guiSkin)
        {
            NetworkDiscovery = networkDiscovery;
        }
    }
}