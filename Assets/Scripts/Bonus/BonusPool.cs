using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;

namespace svtz.Tanks.Bonus
{
    internal sealed class BonusPool : MonoMemoryPool<Vector2, Bonus>
    {
        private readonly Dictionary<Vector2, Bonus> _bonuses = new Dictionary<Vector2, Bonus>();

        protected override void Reinitialize(Vector2 position, Bonus item)
        {
            base.Reinitialize(position, item);
            item.transform.position = position;
            _bonuses.Add(position, item);
            if (NetworkServer.active)
                NetworkServer.Spawn(item.gameObject);
        }

        protected override void OnDespawned(Bonus item)
        {
            base.OnDespawned(item);
            _bonuses.Remove(item.transform.position);

            if (NetworkServer.active)
                NetworkServer.UnSpawn(item.gameObject);
        }

        public Bonus GetOrSpawn(Vector2 position)
        {
            Bonus bonus;
            if (!_bonuses.TryGetValue(position, out bonus))
            {
                bonus = Spawn(position);
            }

            return bonus;
        }

        public sealed class Client
        {
            private readonly BonusPool _pool;

            public Client(BonusPool pool, GameObject prefab)
            {
                _pool = pool;

                ClientScene.RegisterPrefab(prefab, SpawnHandler, UnspawnHandler);
            }

            private void UnspawnHandler(GameObject spawned)
            {
                if (spawned.activeInHierarchy)
                    _pool.Despawn(spawned.GetComponent<Bonus>());
            }

            private GameObject SpawnHandler(Vector3 position, NetworkHash128 id)
            {
                return _pool.GetOrSpawn(position).gameObject;
            }
        }
    }
}