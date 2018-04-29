using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace svtz.Tanks.Map
{
    internal sealed class MapObjectsManager
    {
        private readonly MapObjectsFactory _moFactory;
        private readonly Dictionary<Vector2, GameObject> _gameObjectsIndex = new Dictionary<Vector2, GameObject>();

        public MapObjectsManager(MapObjectsFactory moFactory)
        {
            _moFactory = moFactory;
        }

        public void Add(Vector2 position, MapObjectKind objectKind)
        {
            var mo = _moFactory.Create(objectKind, position);
            NetworkServer.Spawn(mo);
            _gameObjectsIndex.Add(position, mo);
        }

        public void RemoveAt(Vector2 position)
        {
            GameObject mo;
            if (!_gameObjectsIndex.TryGetValue(position, out mo))
            {
                Debug.LogWarning("Попытка удалить несуществующий объект с карты");
                return;
            }

            NetworkServer.Destroy(mo);
            _gameObjectsIndex.Remove(position);
        }
    }

    internal static class MapObjectsControllerExtensions
    {
        public static void Add(this MapObjectsManager mapObjects, float x, float y, MapObjectKind objectKind)
        {
            mapObjects.Add(new Vector2(x, y), objectKind);
        }
    }
}
