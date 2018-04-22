using svtz.Tanks.Assets.Scripts.Common;
using Zenject;

namespace svtz.Tanks.Assets.Scripts.Map
{
    internal sealed class WallHealth : HealthBase
    {
        private MapObjectsController _mapObjectsController;

        [Inject]
        private void Construct(MapObjectsController mapObjectsController)
        {
            _mapObjectsController = mapObjectsController;
        }

        protected override void OnZeroHealthAtServer()
        {
            _mapObjectsController.RemoveAt(transform.position);
        }

        protected override void OnChangeHealthAtClient(int health)
        {
        }
    }
}