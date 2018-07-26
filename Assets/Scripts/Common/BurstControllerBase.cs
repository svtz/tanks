using UnityEngine;
using Zenject;

namespace svtz.Tanks.Common
{
    public abstract class BurstControllerBase<TBurst> : MonoBehaviour
        where TBurst : BurstControllerBase<TBurst>
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
            _pool.Despawn((TBurst)this);
        }

        private void Reset()
        {
            ParticleSystem.Play();
        }

        public class Pool : MonoMemoryPool<Vector3, TBurst>
        {
            protected override void Reinitialize(Vector3 p1, TBurst item)
            {
                base.Reinitialize(p1, item);
                item.transform.position = p1;
                item.Reset();
            }
        }
    }
}