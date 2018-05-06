using System.Linq;
using svtz.Tanks.Common;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Networking;
using Zenject;
using Debug = UnityEngine.Debug;

namespace svtz.Tanks.Projectile
{
    internal sealed class Projectile : NetworkBehaviour
    {
#pragma warning disable 0649
        public int Damage;
        public float TTL;
        public float Speed;

        public float CastWidth;
        public float[] CastDistances;
#pragma warning restore 0649

        private Rigidbody2D _rb2D;
        private ProjectilePool _pool;
        private DelayedExecutor _delayedExecutor;

        private GameObject _owner;
        private DelayedExecutor.IDelayedTask _autoDespawn;
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

        private bool IsEqualOrChildOfOwner(GameObject obj)
        {
            if (obj == _owner)
                return true;

            if (obj.transform.parent == null)
                return false;

            return IsEqualOrChildOfOwner(obj.transform.parent.gameObject);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!isServer)
                return;

            if (IsEqualOrChildOfOwner(other.gameObject))
                return;

            Vector2 direction = transform.up;
            var perpendicular = Vector2.Perpendicular(direction);

            Debug.DrawLine(_owner.transform.position, transform.position, Color.yellow, 2);

            var shouldDespawn = false;
            foreach (var castDistance in CastDistances)
            {
                var castStart =
                    (Vector2)transform.position
                    + direction * castDistance
                    + perpendicular * CastWidth / 2;

                var castEnd =
                    (Vector2)transform.position
                    + direction * castDistance
                    - perpendicular * CastWidth / 2;

                Debug.DrawLine(castStart, castEnd, Color.magenta, 2);

                var cast = Physics2D.LinecastAll(castStart, castEnd)
                    .Select(h => h.transform)
                    .Where(h => h != transform && h != _owner.transform)
                    .Distinct();

                foreach (var hit in cast)
                {
                    var target = hit.GetComponent<AbstractProjectileTarget>();
                    if (target != null)
                    {
                        target.TakeDamage(Damage, _owner);
                        shouldDespawn = true;
                    }
                }

                if (shouldDespawn)
                {
                    TryDespawn();
                    break;
                }
            }

            if (!shouldDespawn)
            {
                Debug.LogError("Вроде во что-то врезались, а во что - не понятно: " + other.gameObject.name);
                Debug.Break();
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
