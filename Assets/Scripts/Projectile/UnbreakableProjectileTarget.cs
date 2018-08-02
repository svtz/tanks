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

        public override void TakeDamage(float penetration, IPlayer damager)
        {
            _soundEffectsFactory.PlayOnAllClients(transform.position, SoundEffectKind.UnbreakableHit);
            if (penetration >= Durability)
            {
                Destroy(gameObject);
            }
        }
    }
}