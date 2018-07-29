using svtz.Tanks.Common;
using UnityEngine;
using Zenject;

namespace svtz.Tanks.Tank
{
    public sealed class TankExplosionController : BurstControllerBase<TankExplosionController>
    {
        private DelayedExecutor _delayedExecutor;
        private float _animationLenght;

        [Inject]
        private void Construct(DelayedExecutor delayedExecutor)
        {
            _delayedExecutor = delayedExecutor;
        }

        private void Start()
        {
            var stateInfo = GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(0);
            _animationLenght = stateInfo.length;
        }

        protected override void Reset()
        {
            base.Reset();
            _delayedExecutor.Add(OnParticleSystemStopped, _animationLenght);
        }
    }
}    