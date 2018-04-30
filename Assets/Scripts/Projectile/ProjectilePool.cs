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
                // по-хорошему, снар€д на клиенте деспавнитс€ и без нашей помощи.
                // но иногда что-то идЄт не по плану, например:
                // стенка уничтожаетс€ раньше, чем  Ћ»≈Ќ“— »… снар€д прочухает, что врезалс€ в неЄ
                spawned.GetComponent<Projectile>().TryDespawn();
            }

            private GameObject SpawnHandler(Vector3 position, NetworkHash128 id)
            {
                return _pool.Spawn().gameObject;
            }
        }
    }
}