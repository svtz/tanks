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
        public int Damage;
        public float TTL;

        public float CastWidth;
        public float[] CastDistances;

        public float Cooldown;
#pragma warning restore 0649

        protected GameObject Owner { get; private set; }
        protected SoundEffectsFactory SoundEffectsFactory { get; private set; }
        private IPlayer _ownerPlayer;
        private bool _despawned;

        [Inject]
        public void Construct(SoundEffectsFactory soundEffectsFactory)
        {
            SoundEffectsFactory = soundEffectsFactory;
        }

        [ClientRpc]
        private void RpcLaunch(Vector2 position, Quaternion rotation, GameObject owner)
        {
            Owner = owner;
            _ownerPlayer = owner.GetComponent<PlayerInfo>().Player;

            transform.position = position;
            transform.rotation = rotation;

            _despawned = false;

            var ownerIdentity = owner.GetComponent<NetworkIdentity>();
            if (ownerIdentity != null && ownerIdentity.isLocalPlayer)
            {
                SoundEffectsFactory.Play(position, LaunchSound, SoundEffectSource.LocalPlayer);
            }
            else
            {
                SoundEffectsFactory.Play(position, LaunchSound, SoundEffectSource.Environment);
            }

            OnRpcLaunch();
        }

        protected abstract SoundEffectKind LaunchSound { get; }

        protected virtual void OnRpcLaunch()
        {
        }

        public void Launch(Transform spawn, GameObject owner)
        {
            Assert.IsTrue(isServer);
            RpcLaunch(spawn.position, spawn.rotation, owner);
        }

        private bool IsEqualOrChildOfOwner(GameObject obj)
        {
            if (obj == Owner)
                return true;

            if (obj.transform.parent == null)
                return false;

            return IsEqualOrChildOfOwner(obj.transform.parent.gameObject);
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
                    return true;
                }
            }

            return false;
        }

        public void TryDespawn()
        {
            if (!_despawned)
            {
                DoDespawn();
            }

            _despawned = true;
        }

        protected abstract void DoDespawn();
    }
}