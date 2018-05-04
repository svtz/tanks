using svtz.Tanks.Map;

namespace svtz.Tanks.Projectile
{
    internal sealed class RegularWallSegmentProjectileTarget : AbstractProjectileTarget
    {
        private WallHealth _wallHealth;

        private void Start()
        {
            // хотел сделать через Inject, но там бага
            // https://github.com/modesttree/Zenject/issues/275
            _wallHealth = GetComponentInParent<WallHealth>();
        }

        public override void TakeDamage(int amount, string teamId)
        {
            _wallHealth.DestroySegment(gameObject);
        }
    }
}