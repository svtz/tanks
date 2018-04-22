using System;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace svtz.Tanks.Assets.Scripts.Tank
{
    internal sealed class TankFactory : Factory<Vector2, GameObject>
    {
        [Serializable]
        public class Settings
        {
#pragma warning disable 0649
            public GameObject TankPrefab;
#pragma warning restore 0649
        }

        public sealed class TankFactoryImpl : IFactory<Vector2, GameObject>, IValidatable
        {
            private readonly Settings _settings;
            private readonly DiContainer _container;

            public TankFactoryImpl(Settings settings, DiContainer container)
            {
                _settings = settings;
                _container = container;
            }

            public GameObject Create(Vector2 position)
            {
                var tank = _container.InstantiatePrefab(_settings.TankPrefab);
                tank.transform.position = position;
                tank.transform.rotation = GetRandomQuanterion();

                return tank;
            }


            private Quaternion GetRandomQuanterion()
            {
                return Quaternion.Euler(0, 0, Random.Range(0, 4) * 90);
            }

            public void Validate()
            {
                _container.InstantiatePrefab(_settings.TankPrefab);
            }
        }
    }
}
