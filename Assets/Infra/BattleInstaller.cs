using svtz.Tanks.Camera;
using svtz.Tanks.Common;
using svtz.Tanks.Map;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;

namespace svtz.Tanks.Infra
{
    internal sealed class BattleInstaller : MonoInstaller
    {
#pragma warning disable 0649
        public MapCreator.Settings MapCreatorSettings;
        public MapObject.Settings MapObjectSettings;
        public TankSpawner.Settings SpawnControllerSettings;
        public GameObject CameraPrefab;
        public GameObject BackgroundPrefab;
#pragma warning restore 0649

        public override void InstallBindings()
        {
            Container.BindFactory<MapObjectKind, Vector2, MapObject, MapObject.Factory>().WithArguments(MapObjectSettings);

            Container.Bind<MapObjectsManager>().AsSingle();

            Container.Bind<MapParser>().AsSingle();

            Container.Bind<MapCreator>().AsSingle().WithArguments(MapCreatorSettings);

            Container.BindFactory<Background, Background.Factory>().FromComponentInNewPrefab(BackgroundPrefab);

            Container.BindInterfacesAndSelfTo<DelayedExecutor>().AsSingle();

            Container.Bind<TankSpawner>().AsSingle().WithArguments(SpawnControllerSettings);

            Container.Bind<CameraController>().FromComponentInNewPrefab(CameraPrefab).AsSingle().NonLazy();

            Container.Bind<TeamId>().FromComponentInParents();
            Container.Bind<NetworkIdentity>().FromComponentInParents();
            Container.Bind<SpriteRenderer>().FromComponentSibling();
        }
    }
}
