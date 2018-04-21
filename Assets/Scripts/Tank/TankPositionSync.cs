using System;
using UnityEngine;
using UnityEngine.Networking;

namespace svtz.Tanks.Assets.Scripts.Tank
{
    [RequireComponent(typeof(Rigidbody2D), typeof(TankController))]
    internal sealed class TankPositionSync : NetworkBehaviour
    {
        private Direction _currentDirection;
        private Vector2 _remotePosition;
        private Rigidbody2D _rb2D;

        private void Start()
        {
            _rb2D = GetComponent<Rigidbody2D>();
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
        }

        private void FixedUpdate()
        {
            if (isLocalPlayer)
                return;

            var newPosition = transform.position;
            switch (_currentDirection)
            {
                case Direction.XPlus:
                case Direction.XMinus:
                    newPosition.y = _remotePosition.y;
                    _rb2D.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
                    break;

                case Direction.YPlus:
                case Direction.YMinus:
                    newPosition.x = _remotePosition.x;
                    _rb2D.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            transform.position = newPosition;
            transform.rotation = Quaternion.Euler(0, 0, DirectionHelper.Rotations[_currentDirection]);
        }
    }
}
