using svtz.Tanks.Assets.Scripts.Common;
using svtz.Tanks.Assets.Scripts.Map;
using UnityEngine;
using Zenject;

namespace svtz.Tanks.Assets.Scripts.Tank
{
    internal sealed class TankHealth : HealthBase
    {
#pragma warning disable 0649
        public RectTransform HealthBar;
#pragma warning restore 0649

        private TankSpawner _tankSpawner;
        private TankObject _tank;

        [Inject]
        private void Construct(TankSpawner tankSpawner, TankObject tank)
        {
            _tankSpawner = tankSpawner;
            _tank = tank;
        }

        protected override void OnZeroHealthAtServer()
        {
            _tankSpawner.DestroyAndRespawn(connectionToClient, _tank);
        }

        protected override void OnChangeHealthAtClient(int health)
        {
            HealthBar.sizeDelta = new Vector2(
                health,
                HealthBar.sizeDelta.y);
        }
    }
}
