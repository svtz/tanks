using svtz.Tanks.Common;
using svtz.Tanks.Map;
using UnityEngine;
using Zenject;

namespace svtz.Tanks.Tank
{
    internal sealed class TankHealth : HealthBase
    {
#pragma warning disable 0649
        public RectTransform HealthBar;
#pragma warning restore 0649

        private TankSpawner _tankSpawner;

        [Inject]
        private void Construct(TankSpawner tankSpawner)
        {
            _tankSpawner = tankSpawner;
        }

        protected override void OnZeroHealthAtServer()
        {
            _tankSpawner.DestroyAndRespawn(connectionToClient, gameObject);
        }

        protected override void OnChangeHealthAtClient(int health)
        {
            HealthBar.sizeDelta = new Vector2(
                health,
                HealthBar.sizeDelta.y);
        }
    }
}
