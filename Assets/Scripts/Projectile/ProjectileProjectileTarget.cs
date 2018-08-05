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

        protected override void OnHit()
        {
        }

        protected override void OnKilled(IPlayer killer)
        {
            _bulletShot.TryDespawn();
        }
    }
}