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

        public int Penetration;
        public int OverchargedPenetration;

        public AbstractProjectileTarget ProjectileTarget;
#pragma warning restore 0649

        protected GameObject Owner { get; private set; }
        protected SoundEffectsFactory SoundEffectsFactory { get; private set; }
        private IPlayer _ownerPlayer;
        private bool _despawned;
        private float _remainingPenetration;

        [Inject]
        public void Construct(SoundEffectsFactory soundEffectsFactory)
        {
            SoundEffectsFactory = soundEffectsFactory;
        }

        [ClientRpc]
        private void RpcLaunch(Vector2 position, Quaternion rotation, GameObject owner,
            bool speedUp, bool overcharge)
        {
            Owner = owner;
            _ownerPlayer = owner.GetComponent<PlayerInfo>().Player;

            transform.position = position;
            transform.rotation = rotation;

            _despawned = false;

            _remainingPenetration = overcharge ? OverchargedPenetration : Penetration;

            var ownerIdentity = owner.GetComponent<NetworkIdentity>();
            if (ownerIdentity != null && ownerIdentity.isLocalPlayer)
            {
                SoundEffectsFactory.Play(position, LaunchSound, SoundEffectSource.LocalPlayer);
            }
            else
            {
                SoundEffectsFactory.Play(position, LaunchSound, SoundEffectSource.Environment);
            }

            OnRpcLaunch(speedUp, overcharge);
        }

        private bool _speedUp;
        public void Speedup()
        {
            _speedUp = true;
        }

        private bool _overcharge;
        public void Overcharge()
        {
            _overcharge = true;
        }

        protected abstract SoundEffectKind LaunchSound { get; }

        protected abstract void OnRpcLaunch(bool speedUp, bool overcharge);

        public void Launch(Transform spawn, GameObject owner)
        {
            Assert.IsTrue(isServer);
            RpcLaunch(spawn.position, spawn.rotation, owner, _speedUp, _overcharge);
        }

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

        protected bool DamageTarget(Vector2 hitCenter, GameObject hitObject)
        {
            if (Owner != null && IsEqualOrChildOfOwner(hitObject))
                return false;

            if (Owner != null)
                Debug.DrawLine(Owner.transform.position, hitCenter, Color.yellow, 2);

            Vector2 direction = transform.up;
            var perpendicular = Vector2.Perpendicular(direction);

            foreach (var castDistance in CastDistances)
            {
                var hitPoint = hitCenter + direction * castDistance;

                var castStart = hitPoint + perpendicular * CastWidth / 2;
                var castEnd = hitPoint - perpendicular * CastWidth / 2;

                Debug.DrawLine(castStart, castEnd, Color.magenta, 2);

                var splashCast = Physics2D.LinecastAll(castStart, castEnd)
                    .Select(h => h.transform)
                    .Where(h => h != transform && (Owner == null || h != Owner.transform))
                    .Distinct();

                var penetrationLoss = 0.0f;
                var destroySelf = false;
                foreach (var splashHit in splashCast)
                {
                    if (!_hitTargets.Add(splashHit))
                        continue;

                    if (ProjectileTarget != null)
                    {
                        var anotherShot = splashHit.GetComponent<AbstractShot>();
                        if (anotherShot != null && anotherShot._remainingPenetration >= ProjectileTarget.Durability)
                        {
                            destroySelf = true;
                            anotherShot._remainingPenetration -= ProjectileTarget.Durability;
                        }
                    }

                    var splashTarget = splashHit.GetComponent<AbstractProjectileTarget>();
                    if (splashTarget != null)
                    {
                        penetrationLoss = Mathf.Max(penetrationLoss, splashTarget.Durability);
                        splashTarget.TakeDamage(_remainingPenetration, _ownerPlayer);
                    }
                }

                _remainingPenetration -= penetrationLoss;
                if (_remainingPenetration <= 0 || destroySelf)
                    return true;
            }

            return false;
        }

        public void TryDespawn()
        {
            if (!_despawned)
            {
                _overcharge = false;
                _speedUp = false;
                _hitTargets.Clear();

                DoDespawn();
            }

            _despawned = true;
        }

        protected abstract void DoDespawn();
    }
}