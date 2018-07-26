﻿using svtz.Tanks.Audio;
using svtz.Tanks.Bonus;
using svtz.Tanks.Bonus.Impl;
using svtz.Tanks.Camera;
using svtz.Tanks.Common;
using svtz.Tanks.Map;
using svtz.Tanks.Projectile;
using svtz.Tanks.Tank;
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
        public BonusSpawner.Settings BonusSpawnerSettings;
        public GameObject ProjectilePrefab;
        public GameObject ProjectileBurstPrefab;
        public GameObject TankExplosionPrefab;
        public GameObject BonusPrefab;
        public BonusEffects BonusEffects;
        public GameObject SoundEffectsFactoryPrefab;
        public int ProjectilePoolInitialSize;
        public int BonusPoolInitialSize;
        public int SoundEffectsPoolInitialSize;
        public int TankExplosionPoolInitialSize;
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
            Container.Bind<TeamId>().FromComponentInParents(); // inParents - для башни танка
            Container.Bind<NetworkIdentity>().FromComponentInParents(); // inParents - для башни танка
            Container.Bind<TankPositionSync>().FromComponentInParents(); // inParents - для башни танка
            Container.Bind<SpriteRenderer>().FromComponentSibling();
            Container.Bind<Rigidbody2D>().FromComponentSibling();
            Container.Bind<TankController>().FromComponentSibling();
            Container.Bind<MoveSoundController>().FromComponentSibling();
            Container.Bind<TurretController>().FromComponentInChildren();
            Container.Bind<CrawlerBeltsController>().FromComponentInChildren();

            // Звуки
            Container.BindMemoryPool<AudioSource, SoundEffectsFactory.Pool>()
                .WithInitialSize(SoundEffectsPoolInitialSize)
                .ExpandByOneAtATime()
                .FromNewComponentOnNewGameObject();
            Container.Bind<SoundEffectsFactory>().FromComponentInNewPrefab(SoundEffectsFactoryPrefab).AsSingle().NonLazy();

            // Снаряды
            Container.BindMemoryPool<Projectile.Projectile, ProjectilePool>()
                .WithInitialSize(ProjectilePoolInitialSize)
                .ExpandByOneAtATime()
                .FromComponentInNewPrefab(ProjectilePrefab);
            Container.Bind<ProjectilePool.Client>().AsSingle().WithArguments(ProjectilePrefab).NonLazy();

            // Эффекты
            Container.BindMemoryPool<ProjectileBurstController, ProjectileBurstController.Pool>()
                .WithInitialSize(ProjectilePoolInitialSize)
                .ExpandByOneAtATime()
                .FromComponentInNewPrefab(ProjectileBurstPrefab);
            Container.BindMemoryPool<TankExplosionController, TankExplosionController.Pool>()
                .WithInitialSize(TankExplosionPoolInitialSize)
                .ExpandByOneAtATime()
                .FromComponentInNewPrefab(TankExplosionPrefab);

            // Бонусы
            Container.Bind<BonusSpawner>().AsSingle().WithArguments(BonusSpawnerSettings);
            Container.BindMemoryPool<Bonus.Bonus, BonusPool>()
                .WithInitialSize(BonusPoolInitialSize)
                .ExpandByOneAtATime()
                .FromComponentInNewPrefab(BonusPrefab);
            Container.Bind<BonusPool.Client>().AsSingle().WithArguments(BonusPrefab).NonLazy();
            BonusImplementationsInstaller.Install(Container, BonusEffects);
        }
    }
}
