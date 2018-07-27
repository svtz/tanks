using System.Linq;
using svtz.Tanks.Audio;
using svtz.Tanks.BattleStats;
using svtz.Tanks.Common;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Networking;
using Zenject;

namespace svtz.Tanks.Projectile
{
    internal sealed class GaussianShot : NetworkBehaviour
    {
#pragma warning disable 0649
        public int Damage;
        public float TTL;
        public float Speed;

        public float CastWidth;
        public float[] CastDistances;
#pragma warning restore 0649

        private Rigidbody2D _rb2D;
        private GaussianShotPool _pool;
        private DelayedExecutor _delayedExecutor;

        private GameObject _owner;
        private IPlayer _ownerPlayer;
        private DelayedExecutor.IDelayedTask _autoDespawn;
        private ProjectileBurstController.Pool _burstPool;
        private SoundEffectsFactory _soundEffectsFactory;
        private bool _despawned;

        [Inject]
        public void Construct(Rigidbody2D rb2D,
            GaussianShotPool pool,
            DelayedExecutor delayedExecutor,
            ProjectileBurstController.Pool burstPool,
            SoundEffectsFactory soundEffectsFactory)
        {
            _rb2D = rb2D;
            _pool = pool;
            _delayedExecutor = delayedExecutor;
            _burstPool = burstPool;
            _soundEffectsFactory = soundEffectsFactory;
        }

        [ClientRpc]
        private void RpcLaunch(Vector2 position, Quaternion rotation, GameObject owner)
        {
            _owner = owner;
            _ownerPlayer = owner.GetComponent<PlayerInfo>().Player;

            transform.position = position;
            transform.rotation = rotation;
            _rb2D.velocity = transform.up * Speed;

            _despawned = false;
            _autoDespawn = _delayedExecutor.Add(TryDespawn, TTL);

            var ownerIdentity = owner.GetComponent<NetworkIdentity>();
            if (ownerIdentity != null && ownerIdentity.isLocalPlayer)
            {
                _soundEffectsFactory.Play(position, SoundEffectKind.RegularShoot, SoundEffectSource.LocalPlayer);
            }
            else
            {
                _soundEffectsFactory.Play(position, SoundEffectKind.RegularShoot, SoundEffectSource.Environment);
            }
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

            if (_owner != null)
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
                    .Where(h => h != transform && (_owner == null || h != _owner.transform))
                    .Distinct();

                foreach (var hit in cast)
                {
                    var target = hit.GetComponent<AbstractProjectileTarget>();
                    if (target != null)
                    {
                        target.TakeDamage(Damage, _ownerPlayer);
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
                Debug.LogError("¬роде во что-то врезались, а во что - не пон€тно: " + other.gameObject.name);
                Debug.Break();
            }
        }

        public void TryDespawn()
        {
            if (!_despawned)
            {
                _burstPool.Spawn(transform.position);

                _autoDespawn.Cancel();
                _pool.Despawn(this);
            }

            _despawned = true;
        }
    }
}