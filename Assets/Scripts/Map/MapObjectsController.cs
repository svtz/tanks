using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace svtz.Tanks.Assets.Scripts.Map
{
    internal sealed class MapObjectsController
    {
        private readonly MapObjectsFactory _objFactory;
        private readonly Background.Factory _backgroundFactory;
        private readonly Dictionary<Vector2, GameObject> _gameObjectsIndex = new Dictionary<Vector2, GameObject>();

        public MapObjectsController(MapObjectsFactory objFactory, Background.Factory backgroundFactory)
        {
            _objFactory = objFactory;
            _backgroundFactory = backgroundFactory;
        }

        public void SetSize(int width, int height)
        {
            var bg = _backgroundFactory.Create();
            bg.SetSize(width, height);

            NetworkServer.Spawn(bg.gameObject);
        }

        public void Add(Vector2 position, MapObjectKind objectKind)
        {
            var go = _objFactory.Create(objectKind, position);

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
