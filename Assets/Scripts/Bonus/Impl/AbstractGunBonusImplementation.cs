using svtz.Tanks.Audio;
using svtz.Tanks.Tank;
using UnityEngine;
using UnityEngine.Networking;

namespace svtz.Tanks.Bonus.Impl
{
    internal abstract class AbstractGunBonusImplementation : IBonusImplementation
    {
        private readonly SoundEffectsFactory _soundEffectsFactory;

        public abstract BonusKind BonusKind { get; }

        protected AbstractGunBonusImplementation(
            SoundEffectsFactory soundEffectsFactory)
        {
            _soundEffectsFactory = soundEffectsFactory;
        }

        public void Apply(GameObject player)
        {
            Debug.Assert(NetworkServer.active);
            var controller = player.GetComponent<FireController>();

            if (controller == null)
            {
                Debug.LogWarning("Тут предполагался только игрок!");
                return;
            }

            controller.SetGun(CreateGun());

            _soundEffectsFactory.PlayOnSingleClient(
                controller.connectionToClient,
                controller.transform.position,
                SoundEffectKind.BonusPickup);
        }

        protected abstract IGun CreateGun();
    }
}