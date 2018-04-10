using svtz.Tanks.Assets.Scripts.Common;
using UnityEngine;
using UnityEngine.Networking;

namespace svtz.Tanks.Assets.Scripts
{
    internal sealed class TankHealth : HealthBase
    {
#pragma warning disable 0649
        public RectTransform HealthBar;
#pragma warning restore 0649

        protected override void OnZeroHealthAtServer()
        {
            CurrentHealth = MaxHealth;
            RpcRespawn();
        }

        protected override void OnChangeHealthAtClient(int health)
        {
            HealthBar.sizeDelta = new Vector2(
                health,
                HealthBar.sizeDelta.y);
        }

        [ClientRpc]
        private void RpcRespawn()
        {
            if (isLocalPlayer)
            {
                // move back to zero location
                transform.position = Vector3.zero;
            }
        }
    }
}
