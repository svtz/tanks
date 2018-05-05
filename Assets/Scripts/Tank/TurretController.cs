using System.Collections.Generic;
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

        private static readonly Dictionary<int, float?> _anglesWordSpace = new Dictionary<int, float?>
        {
            { 0010, 0 },
            { 1010, 45 },
            { 1000, 90 },
            { 1001, 135 },
            { 0001, 180 },
            { 0110, -45 },
            { 0100, -90 },
            { 0101, -135 }
        };

        private float? GetDesiredAngle()
        {
            var angleKey =
                  (_inputManager.TurretLeft() ? 1000 : 0)
                + (_inputManager.TurretRight() ? 100 : 0)
                + (_inputManager.TurretUp() ? 10 : 0)
                + (_inputManager.TurretDown() ? 1 : 0);

            float? result;
            _anglesWordSpace.TryGetValue(angleKey, out result);

            return result;
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
