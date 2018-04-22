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

        private SpawnController _spawnController;

        [Inject]
        private void Construct(SpawnController spawnController)
        {
            _spawnController = spawnController;
        }

        protected override void OnZeroHealthAtServer()
        {
            _spawnController.DestroyAndRespawn(connectionToClient, gameObject);
        }

        protected override void OnChangeHealthAtClient(int health)
        {
            HealthBar.sizeDelta = new Vector2(
                health,
                HealthBar.sizeDelta.y);
        }
    }
}
