using svtz.Tanks.BattleStats;
using UnityEngine.Networking;
using Zenject;

namespace svtz.Tanks.Tank
{
    internal sealed class TankHealth : NetworkBehaviour
    {
#pragma warning disable 0649
        public int MaxHealth;
#pragma warning restore 0649

        private int _currentHealth;

        private void Start()
        {
            _currentHealth = MaxHealth;
        }

        public void TakeDamage(IPlayer damager)
        {
            if (!isServer)
                return;

            _currentHealth -= 1;
            if (_currentHealth <= 0)
            {
                OnZeroHealthAtServer(damager);
            }
        }
        private TankSpawner _tankSpawner;
        private BattleStatsManager _statsManager;

        [Inject]
        private void Construct(TankSpawner tankSpawner, BattleStatsManager statsManager)
        {
            _tankSpawner = tankSpawner;
            _statsManager = statsManager;
        }

        private void OnZeroHealthAtServer(IPlayer damager)
        {
            var killed = gameObject.GetComponent<PlayerInfo>().Player;
            _statsManager.ServerRegisterFrag(killed, damager);
            _tankSpawner.DestroyAndRespawn(connectionToClient, gameObject);
        }
    }
}
