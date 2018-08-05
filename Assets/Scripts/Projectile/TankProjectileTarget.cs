using svtz.Tanks.Audio;
using svtz.Tanks.BattleStats;
using svtz.Tanks.Common;
using svtz.Tanks.Tank;
using Zenject;

namespace svtz.Tanks.Projectile
{
    internal sealed class TankProjectileTarget : AbstractProjectileTarget
    {
        private TankExplosionController.Pool _explosions;
        private SoundEffectsFactory _soundEffects;
        private TankHealth _health;
        private TeamId _teamId;

        [Inject]
        private void Construct(TeamId teamId, SoundEffectsFactory soundEffects, TankExplosionController.Pool explosions)
        {
            _teamId = teamId;
            _soundEffects = soundEffects;
            _explosions = explosions;
        }

        private void Start()
        {
            // хотел сделать через Inject, но там бага
            // https://github.com/modesttree/Zenject/issues/275
            _health = GetComponentInParent<TankHealth>();
        }

        private void OnDestroy()
        {
            _explosions.Spawn(transform.position);
        }

        protected override void OnHit()
        {
        }

        protected override void OnKilled(IPlayer killer)
        {
            if (_teamId.Id != killer.TeamId)
            {
                _health.TakeDamage(killer);
                _soundEffects.PlayOnAllWithException(_teamId.connectionToClient, transform.position, SoundEffectKind.EnemyDeath);
                _soundEffects.PlayOnSingleClient(_teamId.connectionToClient, transform.position, SoundEffectKind.PlayerDeath);
            }
        }
    }
}