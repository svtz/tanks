using System.Linq;
using svtz.Tanks.Common;
using UnityEngine;
using Zenject;

namespace svtz.Tanks.Tank
{
    internal sealed class TurretController : MonoBehaviour
    {
#pragma warning disable 0649
        public float[] Angles;
        public Transform RotationPoint;
#pragma warning restore 0649

        private InputManager _inputManager;
        private TankPositionSync _tankPositionSync;

        [Inject]
        private void Construct(InputManager inputManager, TankPositionSync tankPositionSync)
        {
            _inputManager = inputManager;
            _tankPositionSync = tankPositionSync;
        }

        private float? GetDesiredAngle()
        {
            if (_inputManager.TurretLeft())
                return 90;
            if (_inputManager.TurrentUpLeft())
                return 45;
            if (_inputManager.TurretUp())
                return 0;
            if (_inputManager.TurrentUpRight())
                return -45;
            if (_inputManager.TurretRight())
                return -90;
            if (_inputManager.TurretDownRight())
                return -135;
            if (_inputManager.TurretDown())
                return 180;
            if (_inputManager.TurretDownLeft())
                return 135;

            return null;
        }

        private bool ValidAngle(float angle)
        {
            var localAngle = Quaternion.Angle(transform.parent.rotation, Quaternion.Euler(0, 0, angle));
            return Angles.Any(a => Mathf.Abs(localAngle - a) < 1);
        }

        private void Update()
        {
            if (!_tankPositionSync.isLocalPlayer)
                return;

            var desiredAngle = GetDesiredAngle();
            if (!desiredAngle.HasValue)
                return;
            
            if (!ValidAngle(desiredAngle.Value))
                return;

            RotateTo(desiredAngle.Value);
            _tankPositionSync.CmdSyncTurretRotation(desiredAngle.Value);
        }

        public void RotateTo(float desiredAngle)
        {
            var currentAngle = transform.rotation.eulerAngles.z;
            transform.RotateAround(RotationPoint.position, Vector3.forward, desiredAngle - currentAngle);
        }
    }
}
