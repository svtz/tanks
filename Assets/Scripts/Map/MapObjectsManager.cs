using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace svtz.Tanks.Map
{
    internal sealed class MapObjectsManager
    {
        private readonly MapObjectsFactory _moFactory;
        private readonly HashSet<GameObject> _mapObjects = new HashSet<GameObject>();

        public MapObjectsManager(MapObjectsFactory moFactory)
        {
            _moFactory = moFactory;
        }

        public void Add(Vector2 position, MapObjectKind objectKind)
        {
            var mos = _moFactory.Create(objectKind, position);
            foreach (var mo in mos)
            {
                NetworkServer.Spawn(mo);
                _mapObjects.Add(mo);
            }
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
