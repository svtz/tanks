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
#pragma warning disable 0649
        public Sprite SpeedBoost;
        public Sprite RegularGun;
        public Sprite GaussGun;
        public Sprite GunBoost;

        public SpriteRenderer IconRenderer;
#pragma warning restore 0649


        private Dictionary<BonusKind, Sprite> _bonusSprites;

        private void Start()
        {
            _bonusSprites = new Dictionary<BonusKind, Sprite>
            {
                { BonusKind.MoveSpeedBoost, SpeedBoost },
                { BonusKind.BulletGun, RegularGun },
                { BonusKind.GaussGun, GaussGun },
                { BonusKind.GunBoost, GunBoost },
            };

            OnClientBonusKindChanged(_bonusKind);
        }

        [SyncVar(hook = "OnClientBonusKindChanged")]
        private BonusKind _bonusKind;

        private void OnClientBonusKindChanged(BonusKind newValue)
        {
            if (_bonusSprites != null)
                IconRenderer.sprite = _bonusSprites[newValue];
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