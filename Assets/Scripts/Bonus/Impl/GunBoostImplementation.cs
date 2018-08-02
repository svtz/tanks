using svtz.Tanks.Audio;
using svtz.Tanks.Tank;
using UnityEngine;
using UnityEngine.Networking;

namespace svtz.Tanks.Bonus.Impl
{
    internal sealed class GunBoostImplementation : IBonusImplementation
    {
        private readonly SoundEffectsFactory _soundEffectsFactory;

        public BonusKind BonusKind { get { return BonusKind.GunBoost; } }

        public GunBoostImplementation(
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

            controller.Empower();

            _soundEffectsFactory.PlayOnSingleClient(
                controller.connectionToClient,
                controller.transform.position,
                SoundEffectKind.BonusPickup);
        }
    }
}