﻿using svtz.Tanks.Assets.Scripts.Map;
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

            Container.Bind<BackgroundSizeController>().FromComponentInNewPrefab(BackgroundPrefab).AsSingle();

            Container.Bind<DelayedExecutor>().AsSingle();
            Container.BindInterfacesTo<DelayedExecutor>().AsTransient();

            Container.BindInstance(SpawnControllerSettings);
            Container.Bind<SpawnController>().AsSingle();
        }
    }
}
