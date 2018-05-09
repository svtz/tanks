using System.Collections.Generic;
using UnityEngine;

namespace svtz.Tanks.Tank
{
    public enum Direction
    {
        XPlus,
        XMinus,
        YPlus,
        YMinus
    }

    internal static class DirectionHelper
    {
        public static readonly Dictionary<Direction, Vector2> Directions =
            new Dictionary<Direction, Vector2>
            {
                {Direction.XPlus, Vector2.right},
                {Direction.XMinus, Vector2.left},
                {Direction.YPlus, Vector2.up},
                {Direction.YMinus, Vector2.down}
            };

        public static readonly Dictionary<Direction, float> Rotations =
            new Dictionary<Direction, float>
            {
                {Direction.XPlus, -90},
                {Direction.XMinus, 90},
                {Direction.YPlus, 0},
                {Direction.YMinus, 180}
            };
    }
}