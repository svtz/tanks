using svtz.Tanks.Assets.Scripts.Common;

namespace svtz.Tanks.Assets.Scripts.Map
{
    internal sealed class WallHealth : HealthBase
    {
        protected override void OnZeroHealthAtServer()
        {
            var map = FindObjectOfType<MapObjectsController>();
            map.Remove(transform.position);
        }

        protected override void OnChangeHealthAtClient(int health)
        {
        }
    }
}