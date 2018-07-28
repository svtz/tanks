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

        public override void TakeDamage(int amount, IPlayer damager)
        {
            _bulletShot.TryDespawn();
        }
    }
}