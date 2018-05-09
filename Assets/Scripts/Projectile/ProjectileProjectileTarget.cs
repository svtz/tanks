using svtz.Tanks.BattleStats;
using svtz.Tanks.Common;

namespace svtz.Tanks.Projectile
{
    internal sealed class ProjectileProjectileTarget : AbstractProjectileTarget
    {
        private Projectile _projectile;


        private void Start()
        {
            _projectile = GetComponent<Projectile>();
        }

        public override void TakeDamage(int amount, IPlayer damager)
        {
            _projectile.TryDespawn();
        }
    }
}