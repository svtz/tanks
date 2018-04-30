using svtz.Tanks.Common;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Networking;
using Zenject;

namespace svtz.Tanks.Projectile
{
    internal sealed class Projectile : NetworkBehaviour
    {
#pragma warning disable 0649
        public int Damage;
        public float TTL;
        public float Speed;
#pragma warning restore 0649

        private Rigidbody2D _rb2D;
        private ProjectilePool _pool;
        private DelayedExecutor _delayedExecutor;
        
        private string _teamId;
        private DelayedExecutor.ICancellable _autoDespawn;
        private bool _despawned;

        [Inject]
        public void Construct(Rigidbody2D rb2D, ProjectilePool pool, DelayedExecutor delayedExecutor)
        {
            _rb2D = rb2D;
            _pool = pool;
            _delayedExecutor = delayedExecutor;
        }

        [ClientRpc]
        private void RpcLaunch(Vector2 position, Quaternion rotation, string teamId)
        {
            _teamId = teamId;

            transform.position = position;
            transform.rotation = rotation;
            _rb2D.velocity = transform.up * Speed;

            _despawned = false;
            _autoDespawn = _delayedExecutor.Add(TryDespawn, TTL);
        }

        public void Launch(Transform spawn, TeamId teamId)
        {
            Assert.IsTrue(isServer);
            RpcLaunch(spawn.position, spawn.rotation, teamId.Id);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            var hit = collision.gameObject;
            var teamId = hit.GetComponent<TeamId>();

            if (teamId == null || teamId.Id != _teamId)
            {
                var health = hit.GetComponent<HealthBase>();
                if (health != null)
                {
                    health.TakeDamage(Damage);
                }

                TryDespawn();
            }
        }

        public void TryDespawn()
        {
            if (!_despawned)
            {
                _autoDespawn.Cancel();
                _pool.Despawn(this);
            }

            _despawned = true;
        }
    }
}
