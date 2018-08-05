using svtz.Tanks.Common;
using svtz.Tanks.Projectile;

namespace svtz.Tanks.Tank
{
    internal sealed class BulletGun : AbstractGun<BulletShot, BulletShotPool>
    {
        public BulletGun(BulletShotPool pool, DelayedExecutor delayedExecutor)
            : base(pool, delayedExecutor)
        {
        }

        protected override ShotModifiers GetDefaultShotModifiers()
        {
            return ShotModifiers.Empty;
        }
    }
}