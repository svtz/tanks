using UnityEngine;
using Zenject;

namespace svtz.Tanks.Projectile
{
    public sealed class BurstController : MonoBehaviour
    {
#pragma warning disable 0649
        public ParticleSystem ParticleSystem;
#pragma warning restore 0649

        private Pool _pool;

        [Inject]
        private void Construct(Pool pool)
        {
            _pool = pool;
        }

        public void OnParticleSystemStopped()
        {
            _pool.Despawn(this);
        }

        private void Reset()
        {
            ParticleSystem.Play();
        }

        public class Pool : MonoMemoryPool<Vector3, BurstController>
        {
            protected override void Reinitialize(Vector3 p1, BurstController item)
            {
                base.Reinitialize(p1, item);
                item.transform.position = p1;
                item.Reset();
            }
        }
    }
}