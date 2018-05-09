using svtz.Tanks.Network;
using Zenject;

namespace svtz.Tanks.UserInterface.States
{
    internal abstract class NetworkMenuGUIState : AbstractGUIState
    {
        protected CustomNetworkDiscovery NetworkDiscovery { get; private set; }

        [Inject]
        protected void Construct(CustomNetworkDiscovery networkDiscovery)
        {
            NetworkDiscovery = networkDiscovery;
        }
    }
}