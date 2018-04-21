using svtz.Tanks.Assets.Scripts.Common;
using svtz.Tanks.Assets.Scripts.Map;
using UnityEngine;

namespace svtz.Tanks.Assets.Scripts.Tank
{
    internal sealed class TankHealth : HealthBase
    {
#pragma warning disable 0649
        public RectTransform HealthBar;
#pragma warning restore 0649

        protected override void OnZeroHealthAtServer()
        {
            var spawnController = FindObjectOfType<SpawnController>();
            spawnController.DestroyAndRespawn(connectionToClient, gameObject);
        }

        protected override void OnChangeHealthAtClient(int health)
        {
            HealthBar.sizeDelta = new Vector2(
                health,
                HealthBar.sizeDelta.y);
        }
    }
}
