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

        private BonusPool _pool;
        private Dictionary<BonusKind, IBonusImplementation> _implementations;

        [Inject]
        private void Construct(BonusPool pool, List<IBonusImplementation> implementations)
        {
            _pool = pool;
            _implementations = implementations.ToDictionary(b => b.BonusKind);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (isServer)
            {
                var target = collision.gameObject;
                _implementations[_bonusKind].Apply(target);
                _pool.Despawn(this);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}