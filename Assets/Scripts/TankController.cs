using System;
using UnityEngine;
using UnityEngine.Networking;

public class TankController : NetworkBehaviour
{
    public float Power;
    public float Friction;

    private Rigidbody2D _rb2d;
    private Direction _currentDirection;
    private const float _epsilon = 0.1f;

    private enum Direction
    {
        None,
        XPlus,
        XMinus,
        YPlus,
        YMinus
    }

    private const string AxisHorizontal = "Horizontal";
    private const string AxisVertical = "Vertical";

    // Use this for initialization
    protected virtual void Start()
    {
        if (!isLocalPlayer)
            return;

        _rb2d = GetComponent<Rigidbody2D>();
    }

    //FixedUpdate is called at a fixed interval and is independent of frame rate. Put physics code here.
    protected virtual void FixedUpdate()
    {
        if (!isLocalPlayer)
            return;

        var moveHorizontal = Input.GetAxis(AxisHorizontal);
        var moveVertical = Input.GetAxis(AxisVertical);
        var newDirection = GetDirection(moveHorizontal, moveVertical);

        RotateToNewDirectionIfNeeded(newDirection);
        ApplyFriction();

        Vector2 movement;
        switch (_currentDirection)
        {
            case Direction.None:
                movement = Vector2.zero;
                break;

            case Direction.XPlus:
            case Direction.XMinus:
                movement = new Vector2(moveHorizontal, 0);
                break;

            case Direction.YPlus:
            case Direction.YMinus:
                movement = new Vector2(0, moveVertical);
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }

        _rb2d.AddForce(movement * Power);
    }

    private void ApplyFriction()
    {
        var velocity = _rb2d.velocity;
        var frictionNormalized = -velocity.normalized;
        var friction = frictionNormalized * _rb2d.mass * Friction;

        var velocityMagnitude = velocity.magnitude;
        if (friction.magnitude > velocityMagnitude)
        {
            _rb2d.AddForce(frictionNormalized * velocityMagnitude);
        }
        else
        {
            _rb2d.AddForce(friction);
        }
    }

    private void RotateToNewDirectionIfNeeded(Direction newDirection)
    {
        if (newDirection == _currentDirection || newDirection == Direction.None)
        {
            return;
        }

        var newVelocityX = 0.0f;
        var newVelocityY = 0.0f;
        var newX = transform.position.x;
        var newY = transform.position.y;
        float angle;
        var currentVelocity = _rb2d.velocity;
        switch (newDirection)
        {
            case Direction.XPlus:
                newVelocityX = currentVelocity.magnitude;
                newY = Mathf.Round(newY / Constants.GirdSize) * Constants.GirdSize;
                angle = 90;
                break;
            case Direction.XMinus:
                newVelocityX = -currentVelocity.magnitude;
                newY = Mathf.Round(newY / Constants.GirdSize) * Constants.GirdSize;
                angle = -90;
                break;
            case Direction.YPlus:
                newVelocityY = currentVelocity.magnitude;
                newX = Mathf.Round(newX / Constants.GirdSize) * Constants.GirdSize;
                angle = 0;
                break;
            case Direction.YMinus:
                newVelocityY = -currentVelocity.magnitude;
                newX = Mathf.Round(newX / Constants.GirdSize) * Constants.GirdSize;
                angle = 180;
                break;

            case Direction.None:
            default:
                throw new ArgumentOutOfRangeException();
        }

        _currentDirection = newDirection;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, -angle));
        transform.position = new Vector3(newX, newY);
        _rb2d.velocity = new Vector2(newVelocityX, newVelocityY);
    }

    private Direction GetDirection(float x, float y)
    {
        if (Math.Abs(x - y) < _epsilon)
        {
            return Direction.None;
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