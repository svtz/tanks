using svtz.Tanks.Map;
using UnityEngine;

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

        public override void TakeDamage(int amount, GameObject damager)
        {
            _wallHealth.DestroySegment(gameObject);
        }
    }
}