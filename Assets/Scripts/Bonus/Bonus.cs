using System.Collections.Generic;
using System.Linq;
using svtz.Tanks.Bonus.Impl;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;

namespace svtz.Tanks.Bonus
{
    internal sealed class Bonus : NetworkBehaviour
    {
        [SyncVar(hook = "OnClientBonusKindChanged")]
        private BonusKind _bonusKind;

        private void OnClientBonusKindChanged(BonusKind newValue)
        {
            //todo менять картинку
        }

        public void ServerChangeBonusKind(BonusKind newValue)
        {
            _bonusKind = newValue;
        }

        private BonusSpawner _spawner;
        private Dictionary<BonusKind, IBonusImplementation> _implementations;

        [Inject]
        private void Construct(BonusSpawner spawner,
            List<IBonusImplementation> implementations)
        {
            _spawner = spawner;
            _implementations = implementations.ToDictionary(b => b.BonusKind);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (isServer)
            {
                var target = other.gameObject;
                _implementations[_bonusKind].Apply(target);
                _spawner.ServerRespawnPicked(this);
            }
        }
    }
}