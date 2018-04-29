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
        public MapObjectsFactory.Settings MapObjectsSettings;
        public TankSpawner.Settings SpawnControllerSettings;
#pragma warning restore 0649

        public override void InstallBindings()
        {
            // Карта
            Container.Bind<MapObjectsFactory>().AsSingle().WithArguments(MapObjectsSettings);
            Container.Bind<MapObjectsManager>().AsSingle();
            Container.Bind<MapParser>().AsSingle();
            Container.Bind<MapCreator>().AsSingle().WithArguments(MapCreatorSettings);
            Container.Bind<Background>().FromComponentInHierarchy().AsSingle();

            // Сервис отложенного исполнения
            Container.BindInterfacesAndSelfTo<DelayedExecutor>().AsSingle();

            // Спавнер танков игроков
            Container.Bind<TankSpawner>().AsSingle().WithArguments(SpawnControllerSettings);

            // Камера игрока
            Container.Bind<CameraController>().FromComponentInHierarchy().AsSingle();

            // Компоненты объектов
            Container.Bind<TeamId>().FromComponentInParents();
            Container.Bind<NetworkIdentity>().FromComponentInParents();
            Container.Bind<SpriteRenderer>().FromComponentSibling();
        }
    }
}
