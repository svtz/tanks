using svtz.Tanks.BattleStats;
using UnityEngine;

namespace svtz.Tanks.Projectile
{
    internal abstract class AbstractProjectileTarget : MonoBehaviour
    {
        /// <summary> Нанести урон! </summary>
        /// <param name="penetration">Бронебойность</param>
        /// <param name="damager">Игрок, от которого исходит урон</param>
        public abstract void TakeDamage(float penetration, IPlayer damager);

#pragma warning disable 0649
        public int Durability;
#pragma warning restore 0649
    }
}