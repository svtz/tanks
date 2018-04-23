using svtz.Tanks.Common;
using Zenject;

namespace svtz.Tanks.Map
{
    internal sealed class WallHealth : HealthBase
    {
        private MapObjectsManager _mapObjectsManager;

        [Inject]
        private void Construct(MapObjectsManager mapObjectsManager)
        {
            _mapObjectsManager = mapObjectsManager;
        }

        protected override void OnZeroHealthAtServer()
        {
            _mapObjectsManager.RemoveAt(transform.position);
        }

        protected override void OnChangeHealthAtClient(int health)
        {
        }
    }
}