using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityObject = UnityEngine.Object;

namespace svtz.Tanks.Assets.Scripts.Map
{
    internal sealed class MapObjectsController
    {
        [Serializable]
        public class Settings
        {
#pragma warning disable 0649
            public GameObject UnbreakableWallPrefab;
            public GameObject RegularWallPrefab;
#pragma warning restore 0649
        }

        private readonly Settings _settings;

        public MapObjectsController(Settings settings)
        {
            _settings = settings;
        }

        private readonly Dictionary<Vector2, GameObject> _gameObjectsIndex = new Dictionary<Vector2, GameObject>();

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

        public void Add(Vector2 position, MapObjectKind objectKind)
        {
            var prefab = GetMapObjectPrototype(objectKind);
            var go = UnityObject.Instantiate(prefab, position, Quaternion.identity);
            _gameObjectsIndex.Add(position, go);
            NetworkServer.Spawn(go);
        }

        public void RemoveAt(Vector2 position)
        {
            GameObject go;
            if (!_gameObjectsIndex.TryGetValue(position, out go))
            {
                Debug.LogWarning("Попытка удалить несуществующий объект с карты");
                return;
            }

            NetworkServer.Destroy(go);
            _gameObjectsIndex.Remove(position);
        }
    }

    internal static class MapObjectsControllerExtensions
    {
        public static void Add(this MapObjectsController mapObjects, float x, float y, MapObjectKind objectKind)
        {
            mapObjects.Add(new Vector2(x, y), objectKind);
        }
    }
}
