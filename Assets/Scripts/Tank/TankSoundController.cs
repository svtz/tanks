using svtz.Tanks.Audio;
using UnityEngine;
using Zenject;

namespace svtz.Tanks.Tank
{
    internal sealed class TankSoundController : MonoBehaviour
    {
#pragma warning disable 0649
        public AudioSource Source;
        public float PitchMultiplier;
#pragma warning restore 0649

        private TankController _tankController;
        private SoundEffectsFactory _soundEffectsFactory;

        [Inject]
        private void Construct(TankController tankCtrl, SoundEffectsFactory soundEffectsFactory)
        {
            _tankController = tankCtrl;
            _soundEffectsFactory = soundEffectsFactory;
        }

        private void Start()
        {
            if (_tankController.isLocalPlayer)
            {
                _soundEffectsFactory.Play(transform.position, SoundEffectKind.PlayerSpawn, SoundEffectSource.LocalPlayer);
                Source.Play();
            }
            else
            {
                Destroy(this);
            }
        }


        public void SetMoving(bool moving)
        {
            Source.pitch = moving ? _tankController.Speed * PitchMultiplier : 1;
        }

        private void OnDestroy()
        {
            Source.Stop();
        }
    }
}