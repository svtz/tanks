using System;
using System.Collections.Generic;
using System.Linq;
using svtz.Tanks.Audio;
using svtz.Tanks.BattleStats;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Networking;
using Zenject;

namespace svtz.Tanks.Projectile
{
    internal abstract class AbstractShot : NetworkBehaviour
    {
#pragma warning disable 0649
        public float TTL;

        public float CastWidth;
        public float[] CastDistances;

        public float Cooldown;
        public float BoostedCooldown;

        public LayerMask LayerToExclude;

        public AbstractProjectileTarget ProjectileTarget;
#pragma warning restore 0649

        protected GameObject Owner { get; private set; }
        protected SoundEffectsFactory SoundEffectsFactory { get; private set; }
        private IPlayer _ownerPlayer;
        private ShotModifiers _shotModifiers;

        [Inject]
        public void Construct(SoundEffectsFactory soundEffectsFactory)
        {
            SoundEffectsFactory = soundEffectsFactory;
        }

        #region Launch

        public void Launch(Transform spawn, GameObject owner, ShotModifiers shotModifiers)
        {
            Assert.IsTrue(isServer);
            RpcLaunch(spawn.position, spawn.rotation, owner, shotModifiers);
        }

        [ClientRpc]
        private void RpcLaunch(Vector2 position, Quaternion rotation, GameObject owner, ShotModifiers shotModifiers)
        {
            Owner = owner;
            _ownerPlayer = owner.GetComponent<PlayerInfo>().Player;

            transform.position = position;
            transform.rotation = rotation;

            _despawned = false;
            _shotModifiers = shotModifiers;
            _hitTargets.Clear();

            var ownerIdentity = owner.GetComponent<NetworkIdentity>();
            if (ownerIdentity != null && ownerIdentity.isLocalPlayer)
            {
                SoundEffectsFactory.Play(position, LaunchSound, SoundEffectSource.LocalPlayer);
            }
            else
            {
                SoundEffectsFactory.Play(position, LaunchSound, SoundEffectSource.Environment);
            }

            OnRpcLaunch(shotModifiers);
        }

        protected abstract SoundEffectKind LaunchSound { get; }

        protected abstract void OnRpcLaunch(ShotModifiers shotModifiers);

        #endregion


        private bool IsEqualOrChildOfOwner(GameObject obj)
        {
            if (obj == Owner)
                return true;

            if (obj.transform.parent == null)
                return false;

            return IsEqualOrChildOfOwner(obj.transform.parent.gameObject);
        }

        private readonly HashSet<Transform> _hitTargets = new HashSet<Transform>();

        private void FixedUpdate()
        {
            _hitTargets.Clear();
        }

        protected int GetLayerMask()
        {
            return Physics2D.DefaultRaycastLayers & ~LayerToExclude.value;
        }

        protected bool DamageTarget(Vector2 hitCenter, GameObject hitObject)
        {
            if (Owner != null && IsEqualOrChildOfOwner(hitObject))
                return false;

            if (Owner != null)
            {
                Debug.DrawLine(Owner.transform.position, hitCenter, Color.yellow, 2);
            }

            Vector2 direction = transform.up;
            var perpendicular = Vector2.Perpendicular(direction);

            var destroySelf = false;
            var penetrationDistance = _shotModifiers.IsOvercharged() ? 2 : 1;

            foreach (var castDistance in CastDistances)
            {
                var hitPoint = hitCenter + direction * castDistance;

                var castStart = hitPoint + perpendicular * CastWidth / 2;
                var castEnd = hitPoint - perpendicular * CastWidth / 2;

                Debug.DrawLine(castStart, castEnd, Color.magenta, 2);
                
                var splashCast = Physics2D.LinecastAll(castStart, castEnd, GetLayerMask())
                    .Select(h => h.transform)
                    .Where(h => h != transform && (Owner == null || h != Owner.transform))
                    .Distinct();

                var hit = false;

                foreach (var splashHit in splashCast)
                {
                    if (!_hitTargets.Add(splashHit))
                        continue;

                    if (ProjectileTarget != null)
                    {
                        var another = splashHit.GetComponent<AbstractShot>();
                        if (another != null)
                        {
                            // если другой объект - также чей-то выстрел, то надо проверить его урон по нам.
                            // возможно, это разрушит наш снаряд.
                            ProjectileTarget.TakeDamage(another._shotModifiers, another._ownerPlayer);
                            // результат не проверяем, т.к. код ниже должен сам с этим справиться
                        }
                    }

                    var splashTarget = splashHit.GetComponent<AbstractProjectileTarget>();
                    if (splashTarget != null)
                    {
                        var damageResult = splashTarget.TakeDamage(_shotModifiers, _ownerPlayer);
                        switch (damageResult)
                        {
                            case TakeDamageResult.DestroyProjectile:
                                destroySelf = true;
                                break;
                            case TakeDamageResult.DontDestroyProjectile:
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        hit = true;
                    }
                }

                if (hit)
                    penetrationDistance--;

                if (penetrationDistance <= 0 && destroySelf)
                {
                    return true;
                }
            }

            return destroySelf;
        }


        #region Despawn

        private bool _despawned;

        public void TryDespawn()
        {
            if (!_despawned)
            {
                DoDespawn();
            }

            _despawned = true;
        }

        protected abstract void DoDespawn();

        #endregion
    }
}