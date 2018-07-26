using System.Collections.Generic;
using svtz.Tanks.Audio;
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
        private readonly SoundEffectsFactory _soundEffectsFactory;

        public BonusKind BonusKind { get { return BonusKind.MoveSpeedBoost; } }

        public MoveSpeedBoostImplementation(DelayedExecutor delayedExecutor,
            BonusEffects effects,
            SoundEffectsFactory soundEffectsFactory)
        {
            _delayedExecutor = delayedExecutor;
            _effects = effects;
            _soundEffectsFactory = soundEffectsFactory;
        }

        private readonly Dictionary<TankController, DelayedExecutor.IDelayedTask> _appliedBonuses
             = new Dictionary<TankController, DelayedExecutor.IDelayedTask>();

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
            if (_appliedBonuses.TryGetValue(controller, out appliedBonus))
            {
                appliedBonus.TimeRemaining = _effects.MoveSpeedBonusDuration;
            }
            else
            {
                var oldSpeed = controller.Speed;
                controller.Speed = _effects.MoveSpeedBonusValue;
                appliedBonus = _delayedExecutor.Add(() =>
                    {
                        _appliedBonuses.Remove(controller);
                        if (controller != null)
                        {
                            controller.Speed = oldSpeed;
                        }
                    },
                    _effects.MoveSpeedBonusDuration);

                _appliedBonuses.Add(controller, appliedBonus);
            }

            _soundEffectsFactory.PlayOnSingleClient(
                controller.connectionToClient,
                controller.transform.position,
                SoundEffectKind.BonusPickup);
        }
    }
}