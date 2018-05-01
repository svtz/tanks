using svtz.Tanks.Common;
using svtz.Tanks.Network;
using UnityEngine;
using Zenject;

namespace svtz.Tanks.Infra
{
    internal sealed class ProjectInstaller : MonoInstaller
    {
#pragma warning disable 0649
        public GUISkin GuiSkin;
        public GameObject NetworkManagerPrefab;
        public GameObject NetworkDiscoveryPrefab;
#pragma warning restore 0649

        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<TeamManager>().AsSingle();

            Container.BindInstance(GuiSkin);

            Container.DeclareSignal<ConnectedToServerSignal>();

            Container.Bind<CustomNetworkManager>().FromComponentInNewPrefab(NetworkManagerPrefab).AsSingle();
            Container.Bind<CustomNetworkDiscovery>().FromComponentInNewPrefab(NetworkDiscoveryPrefab).AsSingle();
            Container.Bind<CustomNetworkManagerHUD>()
                .FromNewComponentOnNewGameObject()
                .WithGameObjectName("GUI")
                .AsSingle()
                .NonLazy();
        }
    }
}
