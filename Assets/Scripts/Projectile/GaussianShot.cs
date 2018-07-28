using System;
using System.Linq;
using System.Runtime.InteropServices;
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

        public LineRenderer LineRenderer;

        public float Cooldown;
        public float ShotDelay;
#pragma warning restore 0649

        private GaussianShotPool _pool;
        private DelayedExecutor _delayedExecutor;

        private GameObject _owner;
        private IPlayer _ownerPlayer;
        private DelayedExecutor.IDelayedTask _autoDespawn;
        private GaussianBurstController.Pool _burstPool;
        private SoundEffectsFactory _soundEffectsFactory;
        private bool _despawned;
        private float _currentDelay;
        private bool _isLocalPlayer;

        [Inject]
        public void Construct(
            GaussianShotPool pool,
            DelayedExecutor delayedExecutor,
            GaussianBurstController.Pool burstPool,
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
            
            LineRenderer.positionCount = 0;
            _currentDelay = 0;
            _autoDespawn = null;

            var ownerIdentity = owner.GetComponent<NetworkIdentity>();
            if (ownerIdentity != null && ownerIdentity.isLocalPlayer)
            {
                _isLocalPlayer = true;
                _soundEffectsFactory.Play(position, SoundEffectKind.GaussianCharge, SoundEffectSource.LocalPlayer);
            }
            else
            {
                _isLocalPlayer = false;
                _soundEffectsFactory.Play(position, SoundEffectKind.GaussianCharge, SoundEffectSource.Environment);
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

        private void Update()
        {
            if (_owner != null)
            {
                transform.position = _owner.transform.position;
            }
            else
            {
                if (isServer)
                {
                    TryDespawn();
                }
            }

            _currentDelay += Time.deltaTime;
            if (_currentDelay > ShotDelay && _autoDespawn == null)
                _autoDespawn = _delayedExecutor.Add(TryDespawn, TTL);
        }

        private bool _calculated;
        private void FixedUpdate()
        {
            if (!isServer || _currentDelay < ShotDelay || _calculated)
                return;

            try
            {
                var longRangeCast = Physics2D.BoxCastAll(transform.position,
                    BoxCastSize, 
                    transform.rotation.eulerAngles.z,
                    transform.up, MaxDistance)
                    .OrderBy(hit => hit.distance);

                foreach (var hit in longRangeCast)
                {
                    if (_owner != null && IsEqualOrChildOfOwner(hit.transform.gameObject))
                        continue;

                    if (_owner != null)
                        Debug.DrawLine(_owner.transform.position, hit.centroid, Color.yellow, 2);

                    Vector2 direction = transform.up;
                    var perpendicular = Vector2.Perpendicular(direction);

                    foreach (var castDistance in CastDistances)
                    {
                        var hitPoint = hit.centroid + direction * castDistance;

                        var castStart = hitPoint + perpendicular * CastWidth / 2;
                        var castEnd = hitPoint - perpendicular * CastWidth / 2;

                        Debug.DrawLine(castStart, castEnd, Color.magenta, 2);

                        var splashCast = Physics2D.LinecastAll(castStart, castEnd)
                            .Select(h => h.transform)
                            .Where(h => h != transform && (_owner == null || h != _owner.transform))
                            .Distinct();

                        var hitTheTarget = false;
                        foreach (var splashHit in splashCast)
                        {
                            var splashTarget = splashHit.GetComponent<AbstractProjectileTarget>();
                            if (splashTarget != null)
                            {
                                splashTarget.TakeDamage(Damage, _ownerPlayer);
                                hitTheTarget = true;
                            }
                        }

                        if (hitTheTarget)
                        {
                            RpcShot(transform.position, hitPoint);
                            return;
                        }
                    }
                }
            }
            finally
            {
                _calculated = true;
            }
        }

        [ClientRpc]
        private void RpcShot(Vector2 from, Vector2 to)
        {
            LineRenderer.positionCount = 2;
            LineRenderer.SetPosition(0, from);
            LineRenderer.SetPosition(1, to);
            _burstPool.Spawn(to);

            _soundEffectsFactory.Play(to, SoundEffectKind.GaussianShot,
                _isLocalPlayer ? SoundEffectSource.LocalPlayer : SoundEffectSource.Environment);
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