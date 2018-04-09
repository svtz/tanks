using UnityEngine;
using UnityEngine.Networking;

namespace svtz.Tanks.Assets.Scripts
{
    internal sealed class Health : NetworkBehaviour
    {
        public int MaxHealth = 100;

        public RectTransform HealthBar;

        [SyncVar(hook = "OnChangeHealth")]
        private int _currentHealth;

        private void Start()
        {
            _currentHealth = MaxHealth;
        }

        public void TakeDamage(int amount)
        {
            if (!isServer)
                return;

            _currentHealth -= amount;
            if (_currentHealth <= 0)
            {
                _currentHealth = MaxHealth;
                Debug.Log("Dead!");
                RpcRespawn();
            }
        }

        private void OnChangeHealth(int health)
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
