using svtz.Tanks.Audio;
using svtz.Tanks.Common;
using UnityEngine;
using Zenject;

namespace svtz.Tanks.Projectile
{
    internal sealed class BulletShot : AbstractShot
    {
#pragma warning disable 0649
        public float Speed;
#pragma warning restore 0649

        private Rigidbody2D _rb2D;
        private DelayedExecutor _delayedExecutor;
        private BulletBurstController.Pool _burstPool;
        private BulletShotPool _pool;
        private DelayedExecutor.IDelayedTask _autoDespawn;

        [Inject]
        public void Construct(Rigidbody2D rb2D,
            DelayedExecutor delayedExecutor,
            BulletShotPool pool,
            BulletBurstController.Pool burstPool)
        {
            _rb2D = rb2D;
            _delayedExecutor = delayedExecutor;
            _burstPool = burstPool;
            _pool = pool;
        }

        protected override void OnRpcLaunch()
        {
            base.OnRpcLaunch();
            _rb2D.velocity = transform.up * Speed;
            _autoDespawn = _delayedExecutor.Add(TryDespawn, TTL);
        }

        protected override SoundEffectKind LaunchSound
        {
            get { return SoundEffectKind.RegularShoot; }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!isServer)
                return;

            if (DamageTarget(transform.position, other.gameObject))
            {
                TryDespawn();
            }
        }
        
        protected override void DoDespawn()
        {
            _autoDespawn.Cancel();
            _burstPool.Spawn(transform.position);
            _pool.Despawn(this);
        }
    }
}
