using UnityEngine;
using UnityEngine.Networking;
using Zenject;

namespace svtz.Tanks.Projectile
{
    internal sealed class GaussianShotPool : MonoMemoryPool<GaussianShot>
    {
        protected override void OnSpawned(GaussianShot item)
        {
            base.OnSpawned(item);
            if (NetworkServer.active)
                NetworkServer.Spawn(item.gameObject);
        }

        protected override void OnDespawned(GaussianShot item)
        {
            base.OnDespawned(item);
            if (NetworkServer.active)
                NetworkServer.UnSpawn(item.gameObject);
        }

        public sealed class Client
        {
            private readonly GaussianShotPool _pool;

            public Client(GaussianShotPool pool, GameObject prefab)
            {
                _pool = pool;

                ClientScene.RegisterPrefab(prefab, SpawnHandler, UnspawnHandler);
            }

            private void UnspawnHandler(GameObject spawned)
            {
                spawned.GetComponent<GaussianShot>().TryDespawn();
            }

            private GameObject SpawnHandler(Vector3 position, NetworkHash128 id)
            {
                return _pool.Spawn().gameObject;
            }
        }
    }
}