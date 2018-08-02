using svtz.Tanks.Audio;
using svtz.Tanks.Common;
using UnityEngine;
using Zenject;

namespace svtz.Tanks.Projectile
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(ProjectileProjectileTarget))]
    internal sealed class BulletShot : AbstractShot
    {
#pragma warning disable 0649
        public float Speed;
        public float BoostedSpeed;
        public SpriteRenderer SpriteRenderer;
        public GameObject OverchargeParticles;
        public int Durability;
        public int OverchargedDurability;
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

        protected override void OnRpcLaunch(bool speedUp, bool overcharge)
        {
            _rb2D.velocity = transform.up * (speedUp ? BoostedSpeed : Speed);
            _autoDespawn = _delayedExecutor.Add(TryDespawn, TTL);

            SpriteRenderer.color = overcharge
                ? new Color(1.0f, 0.5f, 0.75f, 1.0f)
                : Color.white;

            if (overcharge)
            {
                OverchargeParticles.GetComponent<ParticleSystem>().Play();
            }

            ProjectileTarget.Durability = overcharge
                ? OverchargedDurability
                : Durability;
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
            OverchargeParticles.GetComponent<ParticleSystem>().Stop();
            _burstPool.Spawn(transform.position);
            _pool.Despawn(this);
        }
    }
}
