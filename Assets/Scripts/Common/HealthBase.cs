using UnityEngine.Networking;

namespace svtz.Tanks.Assets.Scripts.Common
{
    internal abstract class HealthBase : NetworkBehaviour
    {
#pragma warning disable 0649
        public int MaxHealth;
#pragma warning restore 0649

        [SyncVar(hook = "OnChangeHealthAtClient")]
        protected int CurrentHealth;

        private void Start()
        {
            CurrentHealth = MaxHealth;
        }

        public void TakeDamage(int amount)
        {
            if (!isServer)
                return;

            CurrentHealth -= amount;
            if (CurrentHealth <= 0)
            {
                OnZeroHealthAtServer();
            }
        }

        protected abstract void OnZeroHealthAtServer();

        protected abstract void OnChangeHealthAtClient(int health);
    }
}