using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;

namespace svtz.Tanks.Projectile
{
    internal sealed class ProjectilePool : MonoMemoryPool<Projectile>
    {
        private readonly Dictionary<GameObject, Projectile> _spawned = new Dictionary<GameObject, Projectile>();

        protected override void OnSpawned(Projectile item)
        {
            base.OnSpawned(item);
            _spawned.Add(item.gameObject, item);
            if (NetworkServer.active)
                NetworkServer.Spawn(item.gameObject);
        }

        protected override void OnDespawned(Projectile item)
        {
            base.OnDespawned(item);
            _spawned.Remove(item.gameObject);
            if (NetworkServer.active)
                NetworkServer.UnSpawn(item.gameObject);
        }

        public sealed class Client
        {
            private readonly ProjectilePool _pool;

            public Client(ProjectilePool pool, GameObject prefab)
            {
                _pool = pool;

                ClientScene.RegisterPrefab(prefab, SpawnHandler, UnspawnHandler);
            }

            private void UnspawnHandler(GameObject spawned)
            {
                var item = _pool._spawned[spawned];
                _pool.Despawn(item);
            }

            private GameObject SpawnHandler(Vector3 position, NetworkHash128 id)
            {
                return _pool.Spawn().gameObject;
            }
        }
    }
}