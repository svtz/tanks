using System;
using svtz.Tanks.Common;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;

namespace svtz.Tanks.Tank
{
    [RequireComponent(typeof(Rigidbody2D), typeof(TankPositionSync))]
    internal sealed class TankController : NetworkBehaviour
    {
#pragma warning disable 0649
        [SyncVar]
        public float Speed;
#pragma warning restore 0649

        private Rigidbody2D _rb2D;
        private TankPositionSync _sync;
        private InputManager _input;
        private CrawlerBeltsController _crawlerBelts;
        private TankSoundController _tankSound;

        [Inject]
        private void Construct(InputManager input, 
            TankPositionSync sync,
            Rigidbody2D rb2D,
            CrawlerBeltsController belts,
            TankSoundController tankSound)
        {
            _input = input;
            _rb2D = rb2D;
            _sync = sync;
            _crawlerBelts = belts;
            _tankSound = tankSound;
        }

        private Direction _currentDirection;
        private Vector2? _targetPosition;

        private const float Epsilon = 0.01f;

        // Use this for initialization
        private void Start()
        {
            if (Speed > Constants.GridSize)
                throw new NotSupportedException("Слишком большая скорость");

            if (Speed <= 0)
                throw new NotSupportedException("Слишком маленькая скорость");
        }

        private bool IsInCollision()
        {
            return _rb2D.IsTouching(new ContactFilter2D());
        }

        private float _inputX = 0.0f;
        private float _inputY = 0.0f;
        private bool _brake = false;

        private bool CurrentlyMoving { get { return _targetPosition != null; } }

        private void Update()
        {
            if (!isLocalPlayer)
                return;

            // пропишем текущий вход управления
            _inputX = 0.0f;
            _inputY = 0.0f;

            var up = _input.Up();
            var down = _input.Down();
            if (up && !down)
                _inputY = 1.0f;
            else if (down && !up)
                _inputY = -1.0f;

            var right = _input.Right();
            var left = _input.Left();
            if (right && !left)
                _inputX = 1.0f;
            else if (left && !right)
                _inputX = -1.0f;

            _brake = _input.Brake();

            _crawlerBelts.SetAnimationState(CurrentlyMoving);
            _tankSound.SetMoving(CurrentlyMoving);
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
            if (!_targetPosition.HasValue || IsInCollision())
            {
                // если игрок жмёт в какую-либо сторону - пытаемся ехать туда
                if (requestedDirection.HasValue)
                {
                    _currentDirection = requestedDirection.Value;

                    if (!_brake)
                    {
                        //задаём цель
                        _targetPosition = SnapToGrid(_rb2D.position +
                                                     DirectionHelper.Directions[_currentDirection] *
                                                     Constants.GridSize);
                        //едем
                        _rb2D.MovePosition(_rb2D.position +
                                           DirectionHelper.Directions[_currentDirection] * Speed * Constants.GridSize);
                    }
                    else
                    {
                        _targetPosition = null;
                    }
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

                var magnitudeExcess = availableMagnitude - requiredMove.magnitude;
                // если за следующий тик мы доедем куда нужно - можно послушать, чего же хочет игрок
                if (magnitudeExcess > 0)
                {
                    // если игрок жмёт в какую-либо сторону - пытаемся ехать туда
                    if (requestedDirection.HasValue)
                    {
                        //поворачиваемся
                        _currentDirection = requestedDirection.Value;

                        if (!_brake)
                        {
                            //едем
                            _rb2D.MovePosition(_targetPosition.Value +
                                               DirectionHelper.Directions[_currentDirection] * magnitudeExcess);

                            //задаём цель
                            _targetPosition = SnapToGrid(_targetPosition.Value +
                                                         DirectionHelper.Directions[_currentDirection] *
                                                         Constants.GridSize);
                        }
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
            _sync.CmdSyncTankPosition(_currentDirection, transform.position, CurrentlyMoving);
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