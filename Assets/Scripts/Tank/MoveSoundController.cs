using UnityEngine;
using Zenject;

namespace svtz.Tanks.Tank
{
    internal sealed class MoveSoundController : MonoBehaviour
    {
#pragma warning disable 0649
        public AudioSource Source;
        public float PitchMultiplier;
#pragma warning restore 0649

        private TankController _tankController;

        [Inject]
        private void Construct(TankController tankCtrl)
        {
            _tankController = tankCtrl;
        }

        private void Start()
        {
            if (_tankController.isLocalPlayer)
                Source.Play();
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