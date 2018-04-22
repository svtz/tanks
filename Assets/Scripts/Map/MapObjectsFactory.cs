using System;
using UnityEngine;
using Zenject;

namespace svtz.Tanks.Assets.Scripts.Map
{
    internal sealed class MapObjectsFactory : Factory<MapObjectKind, Vector2, GameObject>
    {
        [Serializable]
        public class Settings
        {
#pragma warning disable 0649
            public GameObject UnbreakableWallPrefab;
            public GameObject RegularWallPrefab;
#pragma warning restore 0649
        }

        public sealed class MapObjectsFactoryImpl : IFactory<MapObjectKind, Vector2, GameObject>, IValidatable
        {
            private readonly Settings _settings;
            private readonly DiContainer _container;

            public MapObjectsFactoryImpl(Settings settings, DiContainer container)
            {
                _settings = settings;
                _container = container;
            }

            private GameObject GetMapObjectPrototype(MapObjectKind kind)
            {
                switch (kind)
                {
                    case MapObjectKind.RegularWall:
                        return _settings.RegularWallPrefab;
                    case MapObjectKind.UnbreakableWall:
                        return _settings.UnbreakableWallPrefab;
                    default:
                        throw new ArgumentOutOfRangeException("kind", kind, null);
                }
            }

            public GameObject Create(MapObjectKind objectKind, Vector2 position)
            {
                var prefab = GetMapObjectPrototype(objectKind);

                var go = _container.InstantiatePrefab(prefab);
                go.transform.position = position;
                go.transform.rotation = Quaternion.identity;

                return go;
            }

            public void Validate()
            {
                _container.InstantiatePrefab(_settings.RegularWallPrefab);
                _container.InstantiatePrefab(_settings.UnbreakableWallPrefab);
            }
        }
    }
}