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
        public MapObjectsFactory.Settings MapObjectsFactorySettings;
        public SpawnController.Settings SpawnControllerSettings;
        public TankObject.Settings TankObjectSettings;
        public GameObject BackgroundPrefab;
#pragma warning restore 0649

        public override void InstallBindings()
        {
            Container.BindInstance(MapObjectsFactorySettings);
            Container.BindFactory<MapObjectKind, Vector2, GameObject, MapObjectsFactory>()
                     .FromFactory<MapObjectsFactory.MapObjectsFactoryImpl>();

            Container.Bind<MapObjectsController>().AsSingle();

            Container.Bind<MapParser>().AsSingle();

            Container.BindInstance(MapCreatorSettings);
            Container.Bind<MapCreator>().AsSingle();

            Container.BindFactory<Background, Background.Factory>().FromComponentInNewPrefab(BackgroundPrefab);

            Container.BindInterfacesAndSelfTo<DelayedExecutor>().AsSingle();

            Container.BindInstance(TankObjectSettings);
            Container.BindInterfacesAndSelfTo<TankObject>().AsTransient();
            Container.BindFactory<NetworkConnection, Vector2, TankObject, TankObject.Factory>();

            Container.BindInstance(SpawnControllerSettings);
            Container.Bind<SpawnController>().AsSingle();
        }
    }
}
