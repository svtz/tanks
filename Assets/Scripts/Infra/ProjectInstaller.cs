using svtz.Tanks.BattleStats;
using svtz.Tanks.Common;
using svtz.Tanks.Network;
using svtz.Tanks.Tank;
using svtz.Tanks.UserInterface;
using UnityEngine;
using Zenject;

namespace svtz.Tanks.Infra
{
    internal sealed class ProjectInstaller : MonoInstaller
    {
#pragma warning disable 0649
        public GUIInstaller.GUIMenus Menus;
        public GameObject NetworkManagerPrefab;
        public GameObject NetworkDiscoveryPrefab;
#pragma warning restore 0649

        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<TeamManager>().AsSingle();

            Container.DeclareSignal<ConnectedToServerSignal>();
            Container.DeclareSignal<DisconnectedFromServerSignal>();

            Container.DeclareSignal<GameStartedSignal>();
            Container.Bind<GameStartedSignal.ServerToClient>().AsSingle().WithArguments(MessageCodes.GameStarted).NonLazy();

            Container.DeclareSignal<RespawningSignal>();
            Container.Bind<RespawningSignal.ServerToClient>().AsSingle().WithArguments(MessageCodes.Respawning).NonLazy();

            Container.Bind<CustomNetworkManager>().FromComponentInNewPrefab(NetworkManagerPrefab).AsSingle();
            Container.Bind<CustomNetworkDiscovery>().FromComponentInNewPrefab(NetworkDiscoveryPrefab).AsSingle();

            GUIInstaller.Install(Container, Menus);
            BattleStatsInstaller.Install(Container);
        }
    }
}
