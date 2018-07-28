using UnityEngine;
using UnityEngine.Networking;
using Zenject;

namespace svtz.Tanks.Projectile
{
    internal abstract class AbstractShotPool<TShot, TPool> : MonoMemoryPool<TShot>
        where TShot : AbstractShot
        where TPool : AbstractShotPool<TShot, TPool>
    {
        protected override void OnSpawned(TShot item)
        {
            base.OnSpawned(item);
            if (NetworkServer.active)
                NetworkServer.Spawn(item.gameObject);
        }

        protected override void OnDespawned(TShot item)
        {
            base.OnDespawned(item);
            if (NetworkServer.active)
                NetworkServer.UnSpawn(item.gameObject);
        }

        public sealed class Client
        {
            private readonly TPool _pool;

            public Client(TPool pool, GameObject prefab)
            {
                _pool = pool;

                ClientScene.RegisterPrefab(prefab, SpawnHandler, UnspawnHandler);
            }

            private void UnspawnHandler(GameObject spawned)
            {
                spawned.GetComponent<TShot>().TryDespawn();
            }

            private GameObject SpawnHandler(Vector3 position, NetworkHash128 id)
            {
                return _pool.Spawn().gameObject;
            }
        }
    }
}