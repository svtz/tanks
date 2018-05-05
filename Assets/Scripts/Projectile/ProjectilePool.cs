using UnityEngine;
using UnityEngine.Networking;
using Zenject;

namespace svtz.Tanks.Projectile
{
    internal sealed class ProjectilePool : MonoMemoryPool<Projectile>
    {
        protected override void OnSpawned(Projectile item)
        {
            base.OnSpawned(item);
            if (NetworkServer.active)
                NetworkServer.Spawn(item.gameObject);
        }

        protected override void OnDespawned(Projectile item)
        {
            base.OnDespawned(item);
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
                spawned.GetComponent<Projectile>().TryDespawn();
            }

            private GameObject SpawnHandler(Vector3 position, NetworkHash128 id)
            {
                return _pool.Spawn().gameObject;
            }
        }
    }
}