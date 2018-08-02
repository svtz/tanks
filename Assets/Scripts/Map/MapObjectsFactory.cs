using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace svtz.Tanks.Map
{
    internal sealed class MapObjectsFactory
    {
        private readonly IInstantiator _instantiator;

        [Serializable]
        public class Settings
        {
#pragma warning disable 0649
            public GameObject ArmoredWallPrefab;
            public GameObject UnbreakableWallPrefab;
            public GameObject RegularWallPrefab;
            public GameObject TreePrefab;
#pragma warning restore 0649
        }

        private class CreationInfo
        {
            public GameObject Prefab { get; private set; }
            public Vector2 Offset { get; private set; }

            public CreationInfo(GameObject prefab)
            {
                Prefab = prefab;
                Offset = Vector2.zero;
            }

            public CreationInfo(GameObject prefab, Vector2 offset)
            {
                Prefab = prefab;
                Offset = offset;
            }
        }

        private readonly Dictionary<MapObjectKind, CreationInfo[]> _prototypes;

        public MapObjectsFactory(IInstantiator instantiator, Settings settings)
        {
            _instantiator = instantiator;

            _prototypes = new Dictionary<MapObjectKind, CreationInfo[]>
            {
                {
                    MapObjectKind.UnbreakableWall,
                    new[] {new CreationInfo(settings.UnbreakableWallPrefab)}
                },
                {
                    MapObjectKind.ArmoredWall,
                    new[] {new CreationInfo(settings.ArmoredWallPrefab) }
                },
                {
                    MapObjectKind.RegularWall,
                    new[] {new CreationInfo(settings.RegularWallPrefab)}
                },
                {
                    MapObjectKind.Tree,
                    new[] {new CreationInfo(settings.TreePrefab)}
                },
            };
        }

        public IEnumerable<GameObject> Create(MapObjectKind kind, Vector2 position)
        {
            var infos = _prototypes[kind];
            foreach (var info in infos)
            {
                var go = _instantiator.InstantiatePrefab(info.Prefab);
                go.transform.position = position + info.Offset;
                yield return go;
            }
        }
    }
}