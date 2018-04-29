using System;
using UnityEngine;
using Zenject;

namespace svtz.Tanks.Map
{
    internal sealed class MapObjectsFactory
    {
        private readonly IInstantiator _instantiator;
        private readonly Settings _settings;

        [Serializable]
        public class Settings
        {
#pragma warning disable 0649
            public GameObject UnbreakableWallPrefab;
            public GameObject RegularWallPrefab;
#pragma warning restore 0649
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

        public MapObjectsFactory(IInstantiator instantiator, Settings settings)
        {
            _instantiator = instantiator;
            _settings = settings;
        }

        public GameObject Create(MapObjectKind kind, Vector2 position)
        {
            var go = _instantiator.InstantiatePrefab(GetMapObjectPrototype(kind));
            go.transform.position = position;

            return go;
        }
    }
}