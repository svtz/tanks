using System;
using svtz.Tanks.BattleStats;
using UnityEngine;

namespace svtz.Tanks.Projectile
{
    internal enum TakeDamageResult
    {
        DestroyProjectile,
        DontDestroyProjectile
    }

    internal abstract class AbstractProjectileTarget : MonoBehaviour
    {
        /// <summary> Нанести урон! </summary>
        public TakeDamageResult TakeDamage(ShotModifiers shotModifiers, IPlayer killer)
        {
            OnHit();

            var overcharged = shotModifiers.IsOvercharged();
            var piercing = shotModifiers.IsPiercing();

            bool kill;
            switch (ShotArmor)
            {
                case ShotArmorKind.NoArmor:
                    kill = true;
                    break;

                case ShotArmorKind.ShotProof:
                    kill = overcharged;
                    break;

                case ShotArmorKind.Unbreakable:
                    kill = false;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (kill)
            {
                OnKilled(killer);
            }
            else
            {
                // если не пробили - снаряд дальше не летит в любом случае
                return TakeDamageResult.DestroyProjectile;
            }

            if (!piercing)
            {
                // если снаряд не проникающий - дальше не летим в любом случае
                return TakeDamageResult.DestroyProjectile;
            }

            switch (PiercingArmor)
            {
                case PiercingArmorKind.NoArmor:
                    return TakeDamageResult.DontDestroyProjectile;

                case PiercingArmorKind.PiercingProof:
                    return overcharged ? TakeDamageResult.DontDestroyProjectile : TakeDamageResult.DestroyProjectile;

                case PiercingArmorKind.Unpierceable:
                    return TakeDamageResult.DestroyProjectile;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected abstract void OnHit();

        protected abstract void OnKilled(IPlayer killer);

#pragma warning disable 0649
        public ShotArmorKind ShotArmor;
        public PiercingArmorKind PiercingArmor;
#pragma warning restore 0649
    }
}