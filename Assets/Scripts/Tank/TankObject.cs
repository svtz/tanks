using System;
using System.Runtime.Remoting.Messaging;
using svtz.Tanks.Assets.Scripts.Common;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;
using Random = UnityEngine.Random;

namespace svtz.Tanks.Assets.Scripts.Tank
{
    internal sealed class TankObject : GameObjectHolder<TankObject>
    {
        [Serializable]
        public class Settings
        {
#pragma warning disable 0649
            public GameObject TankPrefab;
#pragma warning restore 0649
        }

        private readonly bool _atServer = false;

        public Vector2 Position { get { return GameObject.transform.position; } }

        public TankObject(DiContainer container, Settings settings, NetworkConnection connection, Vector2 position)
            : this(container, settings, position)
        {
            NetworkServer.ReplacePlayerForConnection(connection, GameObject, 0);

            _atServer = true;
        }

        public TankObject(DiContainer container, Settings settings, Vector2 position)
            : base(container, settings.TankPrefab)
        {
            GameObject.transform.position = position;
            GameObject.transform.rotation = GetRandomQuanterion();
        }

        public void Destroy()
        {
            if (_atServer)
                NetworkServer.Destroy(GameObject);
        }


        private static Quaternion GetRandomQuanterion()
        {
            return Quaternion.Euler(0, 0, Random.Range(0, 4) * 90);
        }

        public class ServerFactory : Factory<NetworkConnection, Vector2, TankObject>
        {
            public class Impl : IFactory<NetworkConnection, Vector2, TankObject>
            {
                private readonly DiContainer _container;
                private readonly Settings _settings;

                public Impl(DiContainer container, Settings settings)
                {
                    _container = container;
                    _settings = settings;
                }

                public TankObject Create(NetworkConnection conn, Vector2 position)
                {
                    return new TankObject(_container, _settings, conn, position);
                }
            }
        }

        public class ClientFactory : Factory<Vector2, TankObject>
        {
            public class Impl : IFactory<Vector2, TankObject>
            {
                private readonly DiContainer _container;
                private readonly Settings _settings;

                public Impl(DiContainer container, Settings settings)
                {
                    _container = container;
                    _settings = settings;
                }

                public TankObject Create(Vector2 position)
                {
                    return new TankObject(_container, _settings, position);
                }
            }
        }

        public class ClientSideSpawner
        {
            private readonly ClientFactory _factory;

            public ClientSideSpawner(Settings settings, ClientFactory factory)
            {
                _factory = factory;
                var hash = settings.TankPrefab.GetComponent<NetworkIdentity>().assetId;
                ClientScene.RegisterSpawnHandler(hash, SpawnTank, UnspawnTank);
            }

            private void UnspawnTank(GameObject spawned)
            {
                UnityEngine.Object.Destroy(spawned);
            }

            private GameObject SpawnTank(Vector3 position, NetworkHash128 assetid)
            {
                var tank = _factory.Create(position);
                return tank.GameObject;
            }
        }
    }
}
