using svtz.Tanks.Map;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;

namespace svtz.Tanks.Tank
{
    internal sealed class TankHealth : NetworkBehaviour
    {
#pragma warning disable 0649
        public RectTransform HealthBar;
        public int MaxHealth;
#pragma warning restore 0649

        [SyncVar(hook = "OnChangeHealthAtClient")]
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
                OnZeroHealthAtServer();
            }
        }
        private TankSpawner _tankSpawner;

        [Inject]
        private void Construct(TankSpawner tankSpawner)
        {
            _tankSpawner = tankSpawner;
        }

        private void OnZeroHealthAtServer()
        {
            _tankSpawner.DestroyAndRespawn(connectionToClient, gameObject);
        }

        private void OnChangeHealthAtClient(int health)
        {
            HealthBar.sizeDelta = new Vector2(
                health,
                HealthBar.sizeDelta.y);
        }
    }
}
