using System;
using System.Collections.Generic;
using System.Linq;
using svtz.Tanks.Common;
using UnityEngine;
using UnityEngine.Networking;

namespace svtz.Tanks.Tank
{
    [RequireComponent(typeof(Rigidbody2D), typeof(TankPositionSync))]
    internal sealed class TankController : NetworkBehaviour
    {
#pragma warning disable 0649
        public float Speed;
#pragma warning restore 0649

        private Rigidbody2D _rb2D;
        private TankPositionSync _sync;

        private Direction _currentDirection;
        private Vector2? _targetPosition;

        private const float Epsilon = 0.01f;




        // Use this for initialization
        private void Start()
        {
            _rb2D = GetComponent<Rigidbody2D>();
            _sync = GetComponent<TankPositionSync>();

            if (Speed > Constants.GridSize)
                throw new NotSupportedException("Слишком большая скорость");

            if (Speed <= 0)
                throw new NotSupportedException("Слишком маленькая скорость");
        }


        #region это что-то кривое, но, может, будет работать?
        private readonly HashSet<int> _collisions = new HashSet<int>();

        private void OnCollisionEnter2D(Collision2D collision)
        {
            _collisions.Add(collision.collider.GetInstanceID());
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            var result = _collisions.Remove(collision.collider.GetInstanceID());
            if (!result)
                throw new InvalidOperationException("Выход из коллизии, в которую не входили.");
        }
        #endregion


        private float _inputX = 0.0f;
        private float _inputY = 0.0f;

        private void Update()
        {
            if (!isLocalPlayer)
                return;

            // пропишем текущий вход управления
            _inputX = 0.0f;
            _inputY = 0.0f;

            var up = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
            var down = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);
            if (up && !down)
                _inputY = 1.0f;
            else if (down && !up)
                _inputY = -1.0f;

            var right = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);
            var left = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
            if (right && !left)
                _inputX = 1.0f;
            else if (left && !right)
                _inputX = -1.0f;
        }

        //FixedUpdate is called at a fixed interval and is independent of frame rate. Put physics code here.
        private void FixedUpdate()
        {
            if (!isLocalPlayer)
                return;

            AlignPositionToGrid();

            // куда-то едем
            if (_targetPosition.HasValue)
            {
                Debug.DrawLine(_rb2D.position, _targetPosition.Value, Color.red, Time.deltaTime, false);

                // достигли ли узла сетки?
                var distanceToTarget = (_targetPosition.Value - _rb2D.position).magnitude;
                if (distanceToTarget < Epsilon)
                {
                    // достигли, стоп машина
                    Debug.Log(string.Format("Доехали до цели. InputX={0} InputY={1}", _inputX, _inputY));
                    _targetPosition = null;
                    _rb2D.velocity = Vector2.zero;
                }
            }
            else
            {
                _rb2D.velocity = Vector2.zero;
            }

            var requestedDirection = GetDirection(_inputX, _inputY);

            // позволяем менять направление, если находимся в узле сетки,
            // или если есть коллизии
            if (!_targetPosition.HasValue || _collisions.Any())
            {
                // если игрок жмёт в какую-либо сторону - пытаемся ехать туда
                if (requestedDirection.HasValue)
                {
                    _currentDirection = requestedDirection.Value;

                    //задаём цель
                    _targetPosition = SnapToGrid(_rb2D.position + 
                        DirectionHelper.Directions[_currentDirection] * Constants.GridSize);

                    //едем
                    _rb2D.MovePosition(_rb2D.position + 
                        DirectionHelper.Directions[_currentDirection] * Speed * Constants.GridSize);
                }
                else
                {
                    // стоим на месте
                    _targetPosition = null;
                }
            }
            //иначе просто едем
            else
            {
                var requiredMove = _targetPosition.Value - _rb2D.position;
                var availableMagnitude = Speed * Constants.GridSize;

                var magnitudeDiff = availableMagnitude - requiredMove.magnitude;
                // если за следующий тик мы доедем куда нужно - можно послушать, чего же хочет игрок
                if (magnitudeDiff > 0)
                {
                    // если игрок жмёт в какую-либо сторону - пытаемся ехать туда
                    if (requestedDirection.HasValue)
                    {
                        //поворачиваемся
                        _currentDirection = requestedDirection.Value;

                        //едем
                        _rb2D.MovePosition(_targetPosition.Value + 
                            DirectionHelper.Directions[_currentDirection] * magnitudeDiff);

                        //задаём цель
                        _targetPosition = SnapToGrid(_targetPosition.Value + 
                            DirectionHelper.Directions[_currentDirection] * Constants.GridSize);
                    }
                    else
                    {
                        _rb2D.MovePosition(_rb2D.position + requiredMove);
                    }
                }
                else
                {
                    _rb2D.MovePosition(_rb2D.position + requiredMove.normalized * availableMagnitude);
                }
            }

            _rb2D.MoveRotation(DirectionHelper.Rotations[_currentDirection]);
            _sync.CmdSyncTankPosition(_currentDirection, transform.position);
        }

        private void AlignPositionToGrid()
        {
            switch (_currentDirection)
            {
                case Direction.XPlus:
                case Direction.XMinus:
                    if ((_rb2D.constraints & RigidbodyConstraints2D.FreezePositionY) != RigidbodyConstraints2D.FreezePositionY)
                    {
                        var distanceToGridY = (float)Math.IEEERemainder(_rb2D.position.y, Constants.GridSize);
                        transform.Translate(new Vector2(0, -distanceToGridY), Space.World);
                        _rb2D.constraints = RigidbodyConstraints2D.FreezePositionY;
                    }
                    break;

                case Direction.YPlus:
                case Direction.YMinus:
                    if ((_rb2D.constraints & RigidbodyConstraints2D.FreezePositionX) != RigidbodyConstraints2D.FreezePositionX)
                    {
                        var distanceToGridX = (float)Math.IEEERemainder(_rb2D.position.x, Constants.GridSize);
                        transform.Translate(new Vector2(-distanceToGridX, 0), Space.World);
                        _rb2D.constraints = RigidbodyConstraints2D.FreezePositionX;
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static Vector2 SnapToGrid(Vector2 v)
        {
            var reminderX = (float)Math.IEEERemainder(v.x, Constants.GridSize);
            var reminderY = (float)Math.IEEERemainder(v.y, Constants.GridSize);
            var reminder = new Vector2(reminderX, reminderY);

            return v-reminder;
        }

        private Direction? GetDirection(float x, float y)
        {
            if (Math.Abs(x - y) < Epsilon)
            {
                return null;
            }

            if (Math.Abs(x) > Math.Abs(y))
            {
                return x > 0 ? Direction.XPlus : Direction.XMinus;
            }
            else
            {
                return y > 0 ? Direction.YPlus : Direction.YMinus;
            }
        }
    }
}