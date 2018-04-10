using System;
using System.Collections.Generic;
using svtz.Tanks.Assets.Scripts.Map;
using UnityEngine;
using UnityEngine.Networking;

namespace svtz.Tanks.Assets.Scripts
{
    internal sealed class MapObjectsController : MonoBehaviour
    {
#pragma warning disable 0649
        public GameObject UnbreakableWallPrefab;
        public GameObject RegularWallPrefab;
#pragma warning restore 0649

        private readonly Dictionary<Vector2, GameObject> _gameObjectsIndex = new Dictionary<Vector2, GameObject>();

        private GameObject GetMapObjectPrototype(MapObjectKind kind)
        {
            switch (kind)
            {
                case MapObjectKind.None:
                    return null;
                case MapObjectKind.RegularWall:
                    return RegularWallPrefab;
                case MapObjectKind.UnbreakableWall:
                    return UnbreakableWallPrefab;
                default:
                    throw new ArgumentOutOfRangeException("kind", kind, null);
            }
        }

        public void Add(Vector2 position, MapObjectKind objectKind)
        {
            var prefab = GetMapObjectPrototype(objectKind);
            if (prefab != null)
            {
                var go = Instantiate(prefab, position, Quaternion.identity);
                _gameObjectsIndex.Add(position, go);
                NetworkServer.Spawn(go);
            }
        }

        public void Remove(Vector2 position)
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
