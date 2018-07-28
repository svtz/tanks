using svtz.Tanks.Audio;
using svtz.Tanks.Tank;
using Zenject;

namespace svtz.Tanks.Bonus.Impl
{
    internal sealed class BulletGunBonusImplementation : AbstractGunBonusImplementation
    {
        private readonly IInstantiator _instantiator;

        public BulletGunBonusImplementation(
            SoundEffectsFactory soundEffectsFactory,
            IInstantiator instantiator) : base(soundEffectsFactory)
        {
            _instantiator = instantiator;
        }

        public override BonusKind BonusKind
        {
            get { return BonusKind.BulletGun; }
        }

        protected override IGun CreateGun()
        {
            return _instantiator.Instantiate<BulletGun>();
        }
    }
}