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
        private DelayedExecutor.IDelayedTask _cooldownTask;

        public bool CanFire { get; private set; }

        protected AbstractGun(TPool pool, DelayedExecutor delayedExecutor)
        {
            _pool = pool;
            _delayedExecutor = delayedExecutor;

            CanFire = true;
        }

        public void Reload()
        {
            CanFire = true;
            if (_cooldownTask != null)
                _cooldownTask.Cancel();
        }

        protected abstract ShotModifiers GetDefaultShotModifiers();

        public void Fire(Transform start, GameObject owner, ShotModifiers additionalModifiers)
        {
            Debug.Assert(CanFire);

            var projectile = _pool.Spawn();

            var finalModifiers = GetDefaultShotModifiers() | additionalModifiers;

            // кулдаун
            CanFire = false;
            _cooldownTask = _delayedExecutor.Add(() => CanFire = true,
                finalModifiers.IsCooldownReduced() ? projectile.BoostedCooldown : projectile.Cooldown);

            projectile.Launch(start, owner, finalModifiers);
        }
    }
}