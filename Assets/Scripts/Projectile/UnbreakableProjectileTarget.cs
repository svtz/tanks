using svtz.Tanks.Audio;
using svtz.Tanks.BattleStats;
using Zenject;

namespace svtz.Tanks.Projectile
{
    internal sealed class UnbreakableProjectileTarget : AbstractProjectileTarget
    {
        private SoundEffectsFactory _soundEffectsFactory;

        [Inject]
        private void Construct(SoundEffectsFactory soundEffectsFactory)
        {
            _soundEffectsFactory = soundEffectsFactory;
        }

        protected override void OnHit()
        {
            _soundEffectsFactory.PlayOnAllClients(transform.position, SoundEffectKind.UnbreakableHit);
        }

        protected override void OnKilled(IPlayer killer)
        {
            Destroy(gameObject);
        }
    }
}