using System;
using UnityEngine;
using System.Collections;

public class TankController : MonoBehaviour
{
    public float Acceleration;

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
    void Start()
    {
        _rb2d = GetComponent<Rigidbody2D>();
    }

    //FixedUpdate is called at a fixed interval and is independent of frame rate. Put physics code here.
    void FixedUpdate()
    {
        var moveHorizontal = Input.GetAxis(AxisHorizontal);
        var moveVertical = Input.GetAxis(AxisVertical);
        var newDirection = GetDirection(moveHorizontal, moveVertical);

        RotateToNewDirectionIfNeeded(newDirection);

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

        _rb2d.AddForce(movement * Acceleration);
    }

    private void RotateToNewDirectionIfNeeded(Direction newDirection)
    {
        if (newDirection == _currentDirection || newDirection == Direction.None)
        {
            return;
        }

        var newX = 0.0f;
        var newY = 0.0f;
        float angle;
        var currentVelocity = _rb2d.velocity;
        switch (newDirection)
        {
            case Direction.XPlus:
                newX = currentVelocity.magnitude;
                angle = -90;
                break;
            case Direction.XMinus:
                newX = -currentVelocity.magnitude;
                angle = 90;
                break;
            case Direction.YPlus:
                newY = currentVelocity.magnitude;
                angle = 0;
                break;
            case Direction.YMinus:
                newY = -currentVelocity.magnitude;
                angle = 180;
                break;

            case Direction.None:
            default:
                throw new ArgumentOutOfRangeException();
        }

        _currentDirection = newDirection;
        gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        _rb2d.velocity = new Vector2(newX, newY);
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