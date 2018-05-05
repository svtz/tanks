using System;
using UnityEngine;
using UnityEngine.Networking;

namespace svtz.Tanks.Tank
{
    [RequireComponent(typeof(Rigidbody2D), typeof(TankController))]
    internal sealed class TankPositionSync : NetworkBehaviour
    {
#pragma warning disable 0649
        public Transform Turret;
#pragma warning restore 0649

        private Direction _currentDirection;
        private Vector2 _remotePosition;
        private float _remoteTurretAngle;
        private Rigidbody2D _rb2D;
        private TurretController _turretController;
        private bool _tankActual = false;
        private bool _turretActual = false;

        private void Start()
        {
            _rb2D = GetComponent<Rigidbody2D>();
            _turretController = Turret.GetComponent<TurretController>();
        }

        [Command]
        public void CmdSyncTurretRotation(float angle)
        {
            RpcSyncTurretRotation(angle);
        }

        [ClientRpc]
        private void RpcSyncTurretRotation(float angle)
        {
            if (isLocalPlayer)
                return;

            _remoteTurretAngle = angle;
            _turretActual = true;
        }

        [Command]
        public void CmdSyncTankPosition(Direction direction, Vector2 position)
        {
            RpcSyncTankPosition(direction, position);
        }

        [ClientRpc]
        private void RpcSyncTankPosition(Direction newDirection, Vector2 position)
        {
            if (isLocalPlayer)
                return;

            _currentDirection = newDirection;
            _remotePosition = position;
            _tankActual = true;
        }

        private void FixedUpdate()
        {
            if (isLocalPlayer)
                return;

            if (_tankActual)
            {
                _tankActual = false;

                var newPosition = transform.position;
                switch (_currentDirection)
                {
                    case Direction.XPlus:
                    case Direction.XMinus:
                        newPosition.y = _remotePosition.y;
                        _rb2D.constraints = RigidbodyConstraints2D.FreezePositionY |
                                            RigidbodyConstraints2D.FreezeRotation;
                        break;

                    case Direction.YPlus:
                    case Direction.YMinus:
                        newPosition.x = _remotePosition.x;
                        _rb2D.constraints = RigidbodyConstraints2D.FreezePositionX |
                                            RigidbodyConstraints2D.FreezeRotation;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }

                transform.position = newPosition;
                transform.rotation = Quaternion.Euler(0, 0, DirectionHelper.Rotations[_currentDirection]);
            }

            if (_turretActual)
            {
                _turretActual = false;
                _turretController.RotateTo(_remoteTurretAngle);
            }
        }
    }
}
