using System;
using svtz.Tanks.Assets.Scripts.Common;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;

namespace svtz.Tanks.Assets.Scripts.Map
{
    internal sealed class MapObject : GameObjectHolder<MapObject>
    {
        [Serializable]
        public class Settings
        {
#pragma warning disable 0649
            public GameObject UnbreakableWallPrefab;
            public GameObject RegularWallPrefab;
#pragma warning restore 0649
        }

        private static GameObject GetMapObjectPrototype(MapObjectKind kind, Settings settings)
        {
            switch (kind)
            {
                case MapObjectKind.RegularWall:
                    return settings.RegularWallPrefab;
                case MapObjectKind.UnbreakableWall:
                    return settings.UnbreakableWallPrefab;
                default:
                    throw new ArgumentOutOfRangeException("kind", kind, null);
            }
        }

        public MapObject(DiContainer container, Settings settings, MapObjectKind kind, Vector2 position)
            : base(container, GetMapObjectPrototype(kind, settings))
        {
            GameObject.transform.position = position;
            GameObject.transform.rotation = Quaternion.identity;

            NetworkServer.Spawn(GameObject);
        }

        public void Destroy()
        {
            NetworkServer.Destroy(GameObject);
        }

        public class Factory : Factory<MapObjectKind, Vector2, MapObject> { }
    }
}