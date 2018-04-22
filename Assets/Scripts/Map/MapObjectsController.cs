using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace svtz.Tanks.Assets.Scripts.Map
{
    internal sealed class MapObjectsController
    {
        private readonly MapObject.Factory _objFactory;
        private readonly Background.Factory _backgroundFactory;
        private readonly Dictionary<Vector2, MapObject> _gameObjectsIndex = new Dictionary<Vector2, MapObject>();

        public MapObjectsController(MapObject.Factory objFactory, Background.Factory backgroundFactory)
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
            var obj = _objFactory.Create(objectKind, position);
            _gameObjectsIndex.Add(position, obj);
        }

        public void RemoveAt(Vector2 position)
        {
            MapObject mo;
            if (!_gameObjectsIndex.TryGetValue(position, out mo))
            {
                Debug.LogWarning("Попытка удалить несуществующий объект с карты");
                return;
            }

            mo.Destroy();
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
