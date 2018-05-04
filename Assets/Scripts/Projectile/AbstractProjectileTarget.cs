using UnityEngine;

namespace svtz.Tanks.Projectile
{
    internal abstract class AbstractProjectileTarget : MonoBehaviour
    {
        /// <summary> Нанести урон! </summary>
        /// <param name="amount">Количество</param>
        /// <param name="teamId">Команда, которая наносит урон</param>
        public abstract void TakeDamage(int amount, string teamId);
    }
}