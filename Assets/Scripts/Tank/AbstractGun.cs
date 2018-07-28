using svtz.Tanks.Common;
using svtz.Tanks.Projectile;
using UnityEngine;

namespace svtz.Tanks.Tank
{
    internal abstract class AbstractGun<TShot, TPool> : IGun
        where TShot : AbstractShot
        where TPool : AbstractShotPool<TShot, TPool>
    {
        private readonly TPool _pool;
        private readonly DelayedExecutor _delayedExecutor;

        public bool CanFire { get; private set; }

        protected AbstractGun(TPool pool, DelayedExecutor delayedExecutor)
        {
            _pool = pool;
            _delayedExecutor = delayedExecutor;

            CanFire = true;
        }

        public void Fire(Transform start, GameObject owner)
        {
            Debug.Assert(CanFire);

            var projectile = _pool.Spawn();
            projectile.Launch(start, owner);

            // кулдаун
            CanFire = false;
            _delayedExecutor.Add(() => CanFire = true, projectile.Cooldown);
        }
    }
}