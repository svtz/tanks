using System.Collections;
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

        private GameObject _owner;
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
        private void RpcLaunch(Vector2 position, Quaternion rotation, GameObject owner)
        {
            _owner = owner;

            transform.position = position;
            transform.rotation = rotation;
            _rb2D.velocity = transform.up * Speed;

            _despawned = false;
            _autoDespawn = _delayedExecutor.Add(TryDespawn, TTL);
        }

        public void Launch(Transform spawn, GameObject owner)
        {
            Assert.IsTrue(isServer);
            RpcLaunch(spawn.position, spawn.rotation, owner);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            var hit = collision.gameObject;
            if (hit == _owner)
                return;

            var target = hit.GetComponent<AbstractProjectileTarget>();
            if (target != null)
            {
                if (isServer)
                {
                    target.TakeDamage(Damage, _owner);
                }
                TryDespawn();
            }
        }

        public void TryDespawn()
        {
            if (!_despawned)
            {
                _autoDespawn.Cancel();
                StartCoroutine(DoDespawn());
            }

            _despawned = true;
        }

        /// <summary>
        /// Нам нужно, чтобы отработали коллизии на всех стенках.
        /// А если мы убиваем пулю на первой же коллизии, то остальные стенки не рушатся.
        /// </summary>
        private IEnumerator DoDespawn()
        {
            _rb2D.velocity = Vector2.zero;
            yield return new WaitForFixedUpdate();
            _pool.Despawn(this);
        }
    }
}
