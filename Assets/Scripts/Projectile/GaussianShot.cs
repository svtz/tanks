using System;
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
        public Vector2 BoxCastSize;

        public float MaxDistance;
        public float CastWidth;
        public float[] CastDistances;
#pragma warning restore 0649

        private GaussianShotPool _pool;
        private DelayedExecutor _delayedExecutor;

        private GameObject _owner;
        private IPlayer _ownerPlayer;
        private DelayedExecutor.IDelayedTask _autoDespawn;
        private ProjectileBurstController.Pool _burstPool;
        private SoundEffectsFactory _soundEffectsFactory;
        private bool _despawned;

        [Inject]
        public void Construct(
            GaussianShotPool pool,
            DelayedExecutor delayedExecutor,
            ProjectileBurstController.Pool burstPool,
            SoundEffectsFactory soundEffectsFactory)
        {
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

            _calculated = false;
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

        private bool _calculated;
        private void FixedUpdate()
        {
            if (!isServer || _calculated)
                return;

            try
            {
                var shotCast = Physics2D.BoxCastAll(transform.position,
                    BoxCastSize, 
                    transform.rotation.eulerAngles.z,
                    transform.up, MaxDistance)
                    .OrderBy(hit => hit.distance);

                var hitPoint = Vector2.zero;
                var hasTarget = false;
                foreach (var hit in shotCast)
                {
                    if (_owner != null && IsEqualOrChildOfOwner(hit.transform.gameObject))
                        continue;

                    //todo пробивать насквозь
                    var target = hit.transform.gameObject.GetComponent<AbstractProjectileTarget>();
                    if (target == null)
                        continue;

                    hitPoint = hit.centroid;
                    hasTarget = true;
                    break;
                }
                
                if (!hasTarget)
                    return;

                if (_owner != null)
                    Debug.DrawLine(_owner.transform.position, hitPoint, Color.yellow, 2);


                Vector2 direction = transform.up;
                var perpendicular = Vector2.Perpendicular(direction);
                foreach (var castDistance in CastDistances)
                {
                    var castStart =
                        hitPoint
                        + direction * castDistance
                        + perpendicular * CastWidth / 2;

                    var castEnd =
                        hitPoint
                        + direction * castDistance
                        - perpendicular * CastWidth / 2;

                    Debug.DrawLine(castStart, castEnd, Color.magenta, 2);

                    var cast = Physics2D.LinecastAll(castStart, castEnd)
                        .Select(h => h.transform)
                        .Where(h => h != transform && (_owner == null || h != _owner.transform))
                        .Distinct();

                    var hitTheTarget = false;
                    foreach (var hit in cast)
                    {
                        var target = hit.GetComponent<AbstractProjectileTarget>();
                        if (target != null)
                        {
                            target.TakeDamage(Damage, _ownerPlayer);
                            hitTheTarget = true;
                        }
                    }

                    if (hitTheTarget)
                        break;
                }
            }
            finally
            {
                _calculated = true;
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