using svtz.Tanks.Assets.Scripts.Camera;
using svtz.Tanks.Assets.Scripts.Map;
using svtz.Tanks.Assets.Scripts.Tank;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;

namespace svtz.Tanks.Assets.Scripts.Common
{
    internal sealed class BattleInstaller : MonoInstaller
    {
#pragma warning disable 0649
        public MapCreator.Settings MapCreatorSettings;
        public MapObject.Settings MapObjectSettings;
        public TankSpawner.Settings SpawnControllerSettings;
        public TankObject.Settings TankObjectSettings;
        public GameObject CameraPrefab;
        public GameObject BackgroundPrefab;
#pragma warning restore 0649

        public override void InstallBindings()
        {
            Container.BindInstance(MapObjectSettings);
            Container.BindFactory<MapObjectKind, Vector2, MapObject, MapObject.Factory>();

            Container.Bind<MapObjectsManager>().AsSingle();

            Container.Bind<MapParser>().AsSingle();

            Container.BindInstance(MapCreatorSettings);
            Container.Bind<MapCreator>().AsSingle();

            Container.BindFactory<Background, Background.Factory>().FromComponentInNewPrefab(BackgroundPrefab);

            Container.BindInterfacesAndSelfTo<DelayedExecutor>().AsSingle();

            Container.BindInstance(TankObjectSettings);
            Container.BindFactory<Vector2, TankObject, TankObject.ClientFactory>()
                     .FromFactory<TankObject.ClientFactory.Impl>();
            Container.BindFactory<NetworkConnection, Vector2, TankObject, TankObject.ServerFactory>()
                     .FromFactory<TankObject.ServerFactory.Impl>();
            Container.Bind<TankObject.ClientSideSpawner>().AsSingle().NonLazy();

            Container.BindInstance(SpawnControllerSettings);
            Container.Bind<TankSpawner>().AsSingle();

            Container.Bind<CameraController>().FromComponentInNewPrefab(CameraPrefab).AsSingle().NonLazy();
        }
    }
}
