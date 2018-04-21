using svtz.Tanks.Assets.Scripts.Map;
using UnityEngine;

namespace svtz.Tanks.Assets.Scripts
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