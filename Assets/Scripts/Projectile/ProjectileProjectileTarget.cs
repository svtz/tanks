using svtz.Tanks.BattleStats;

namespace svtz.Tanks.Projectile
{
    internal sealed class ProjectileProjectileTarget : AbstractProjectileTarget
    {
        private BulletShot _bulletShot;

        private void Start()
        {
            _bulletShot = GetComponent<BulletShot>();
        }

        public override void TakeDamage(float penetration, IPlayer damager)
        {
            if (penetration >= Durability)
                _bulletShot.TryDespawn();
        }
    }
}