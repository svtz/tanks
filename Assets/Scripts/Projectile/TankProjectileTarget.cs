using svtz.Tanks.Common;
using svtz.Tanks.Tank;
using Zenject;

namespace svtz.Tanks.Projectile
{
    internal sealed class TankProjectileTarget : AbstractProjectileTarget
    {
        private TankHealth _health;
        private TeamId _teamId;

        [Inject]
        private void Construct(TeamId teamId)
        {
            _teamId = teamId;
        }

        private void Start()
        {
            // хотел сделать через Inject, но там бага
            // https://github.com/modesttree/Zenject/issues/275
            _health = GetComponentInParent<TankHealth>();
        }

        public override void TakeDamage(int amount, string teamId)
        {
            if (_teamId.Id != teamId)
            {
                _health.TakeDamage(amount);
            }
        }
    }
}