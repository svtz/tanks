using UnityEngine;

namespace svtz.Tanks.Projectile
{
    internal sealed class ProjectileProjectileTarget : AbstractProjectileTarget
    {
        private Projectile _projectile;


        private void Start()
        {
            _projectile = GetComponent<Projectile>();
        }

        public override void TakeDamage(int amount, GameObject damager)
        {
            _projectile.TryDespawn();
        }
    }
}