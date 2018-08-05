using svtz.Tanks.Common;
using svtz.Tanks.Projectile;

namespace svtz.Tanks.Tank
{
    internal sealed class GaussGun : AbstractGun<GaussShot, GaussShotPool>
    {
        public GaussGun(GaussShotPool pool, DelayedExecutor delayedExecutor)
            : base(pool, delayedExecutor)
        {
        }

        protected override ShotModifiers GetDefaultShotModifiers()
        {
            return ShotModifiers.PiercingShot;
        }
    }
}