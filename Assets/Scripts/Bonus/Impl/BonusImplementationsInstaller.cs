using Zenject;

namespace svtz.Tanks.Bonus.Impl
{
    internal sealed class BonusImplementationsInstaller : Installer<BonusEffects, BonusImplementationsInstaller>
    {
        private readonly BonusEffects _effects;

        public BonusImplementationsInstaller(BonusEffects effects)
        {
            _effects = effects;
        }

        public override void InstallBindings()
        {
            Container.Bind<IBonusImplementation>()
                .To<MoveSpeedBoostImplementation>()
                .AsSingle()
                .WithArguments(_effects);
        }
    }
}