﻿using System.Collections.Generic;
using svtz.Tanks.Common;
using svtz.Tanks.Tank;
using UnityEngine;
using UnityEngine.Networking;

namespace svtz.Tanks.Bonus.Impl
{
    internal sealed class MoveSpeedBoostImplementation : IBonusImplementation
    {
        private readonly DelayedExecutor _delayedExecutor;
        private readonly BonusEffects _effects;

        public BonusKind BonusKind { get { return BonusKind.MoveSpeedBoost; } }

        public MoveSpeedBoostImplementation(DelayedExecutor delayedExecutor, BonusEffects effects)
        {
            _delayedExecutor = delayedExecutor;
            _effects = effects;
        }

        private readonly Dictionary<GameObject, DelayedExecutor.IDelayedTask> _appliedBonuses
             = new Dictionary<GameObject, DelayedExecutor.IDelayedTask>();

        public void Apply(GameObject player)
        {
            Debug.Assert(NetworkServer.active);
            var controller = player.GetComponent<TankController>();

            if (controller == null)
            {
                Debug.LogWarning("Тут предполагался только игрок!");
                return;
            }

            DelayedExecutor.IDelayedTask appliedBonus;
            if (_appliedBonuses.TryGetValue(player, out appliedBonus))
            {
                appliedBonus.TimeRemaining = _effects.MoveSpeedBonusDuration;
            }
            else
            {
                var oldSpeed = controller.Speed;
                controller.Speed = _effects.MoveSpeedBonusValue;
                appliedBonus = _delayedExecutor.Add(() =>
                    {
                        _appliedBonuses.Remove(controller.gameObject);
                        if (controller.gameObject != null)
                        {
                            controller.Speed = oldSpeed;
                        }
                    },
                    _effects.MoveSpeedBonusDuration);

                _appliedBonuses.Add(player, appliedBonus);
            }
        }
    }
}