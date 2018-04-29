using System;
using svtz.Tanks.Common;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;

namespace svtz.Tanks.Map
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

        public MapObject(Settings settings, MapObjectKind kind, Vector2 position)
        {
            _prefab = GetMapObjectPrototype(kind, settings);
            _parameters = new GameObjectCreationParameters
            {
                Position = position,
                Rotation = Quaternion.identity
            };
        }

        private readonly GameObject _prefab;
        protected override GameObject Prefab { get { return _prefab; } }

        private readonly GameObjectCreationParameters _parameters;
        protected override GameObjectCreationParameters CreationParameters
        {
            get { return _parameters; }
        }

        protected override void OnCreated()
        {
            base.OnCreated();

            NetworkServer.Spawn(GameObject);
        }

        public void Destroy()
        {
            NetworkServer.Destroy(GameObject);
        }

        public class Factory : Factory<MapObjectKind, Vector2, MapObject> { }
    }
}