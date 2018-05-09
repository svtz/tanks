using svtz.Tanks.BattleStats;
using UnityEngine;

namespace svtz.Tanks.Projectile
{
    internal abstract class AbstractProjectileTarget : MonoBehaviour
    {
        /// <summary> Нанести урон! </summary>
        /// <param name="amount">Количество</param>
        /// <param name="damager">Игрок, от которого исходит урон</param>
        public abstract void TakeDamage(int amount, IPlayer damager);
    }
}