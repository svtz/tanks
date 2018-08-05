using svtz.Tanks.Audio;
using svtz.Tanks.BattleStats;
using svtz.Tanks.Map;
using Zenject;

namespace svtz.Tanks.Projectile
{
    internal sealed class RegularWallSegmentProjectileTarget : AbstractProjectileTarget
    {
        private WallHealth _wallHealth;
        private SoundEffectsFactory _soundEffectsFactory;

        [Inject]
        private void Construct(SoundEffectsFactory soundEffectsFactory)
        {
            _soundEffectsFactory = soundEffectsFactory;
        }

        private void Start()
        {
            // хотел сделать через Inject, но там бага
            // https://github.com/modesttree/Zenject/issues/275
            _wallHealth = GetComponentInParent<WallHealth>();
        }

        protected override void OnHit()
        {
            _soundEffectsFactory.PlayOnAllClients(transform.parent.position, SoundEffectKind.RegularWallDestroy);
        }

        protected override void OnKilled(IPlayer killer)
        {
            _wallHealth.DestroySegment(gameObject);
        }
    }
}