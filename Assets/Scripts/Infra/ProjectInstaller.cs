using svtz.Tanks.Audio;
using svtz.Tanks.BattleStats;
using svtz.Tanks.Bonus.Impl;
using svtz.Tanks.Common;
using svtz.Tanks.Map;
using svtz.Tanks.Network;
using svtz.Tanks.Tank;
using svtz.Tanks.UserInterface;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace svtz.Tanks.Infra
{
    internal sealed class ProjectInstaller : MonoInstaller
    {
#pragma warning disable 0649
        public GUIInstaller.GUIMenus Menus;
        public GameObject NetworkManagerPrefab;
        public GameObject NetworkDiscoveryPrefab;
        public GameObject EventSystemPrefab;
#pragma warning restore 0649

        public override void InstallBindings()
        {
            // Менеджер команд
            Container.BindInterfacesAndSelfTo<TeamManager>().AsSingle();

            // всякие сигналы, как локальные, так и серверные
            Container.DeclareSignal<ConnectedToServerSignal>();
            Container.DeclareSignal<DisconnectedFromServerSignal>();
            Container.DeclareSignal<GameStartedSignal>();

            Container.DeclareSignal<RespawningSignal>();
            Container.Bind<RespawningSignal.ServerToClient>().AsSingle().WithArguments(MessageCodes.Respawning).NonLazy();

            Container.DeclareSignal<SoundEffectsFactory.SoundEffectSignal>();
            Container.Bind<SoundEffectsFactory.SoundEffectSignal.ServerToClient>()
                .AsSingle()
                .WithArguments(MessageCodes.MoveSpeedBonusApplied)
                .NonLazy();

            // сеть-мультиплеер
            Container.Bind<CustomNetworkManager>().FromComponentInNewPrefab(NetworkManagerPrefab).AsSingle();
            Container.Bind<CustomNetworkDiscovery>().FromComponentInNewPrefab(NetworkDiscoveryPrefab).AsSingle();

            // ввод
            Container.Bind<InputManager>().AsSingle();
            Container.Bind<EventSystem>().FromComponentInNewPrefab(EventSystemPrefab).AsSingle();

            // принадлежности карты, которые важно получить до начала боя
            Container.DeclareSignal<MapSettingsUpdatedSignal>();
            Container.Bind<MapSettingsUpdatedSignal.ServerToClient>().AsSingle().WithArguments(MessageCodes.MapSettingsUpdate).NonLazy();
            Container.BindInterfacesAndSelfTo<MapSettingsManager>().AsSingle();

            GUIInstaller.Install(Container, Menus);
            BattleStatsInstaller.Install(Container);
        }
    }
}
