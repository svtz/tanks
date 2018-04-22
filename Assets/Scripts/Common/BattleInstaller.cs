using svtz.Tanks.Assets.Scripts.Map;
using svtz.Tanks.Assets.Scripts.Tank;
using UnityEngine;
using Zenject;

namespace svtz.Tanks.Assets.Scripts.Common
{
    internal sealed class BattleInstaller : MonoInstaller
    {
#pragma warning disable 0649
        public MapCreator.Settings MapCreatorSettings;
        public MapObjectsFactory.Settings MapObjectsFactorySettings;
        public SpawnController.Settings SpawnControllerSettings;
        public TankFactory.Settings TankFactorySettings;
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

            Container.Bind<BackgroundSizeController>().FromComponentInNewPrefab(BackgroundPrefab).AsSingle().NonLazy();

            Container.BindInterfacesTo<DelayedExecutor>().AsSingle();

            Container.BindInstance(TankFactorySettings);
            Container.BindFactory<Vector2, GameObject, TankFactory>()
                     .FromFactory<TankFactory.TankFactoryImpl>();

            Container.BindInstance(SpawnControllerSettings);
            Container.Bind<SpawnController>().AsSingle();
        }
    }
}
