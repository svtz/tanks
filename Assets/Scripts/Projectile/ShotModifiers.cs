using System;

namespace svtz.Tanks.Projectile
{
    [Flags]
    internal enum ShotModifiers
    {
        /// <summary> ��� ������������� </summary>
        Empty = 0,

        /// <summary> ��������� </summary>
        OverchargedShot = 1 << 0,

        /// <summary> ����������� �������� </summary>
        PiercingShot = 1 << 1,

        /// <summary> ���������� ������� </summary>
        AcceleratedShot = 1 << 2,

        /// <summary> ���������� �� </summary>
        ReducedCooldown = 1 << 3
    }

    internal static class ShotModifiersExtensions
    {
        public static bool IsOvercharged(this ShotModifiers modifiers)
        {
            return (modifiers & ShotModifiers.OverchargedShot) != 0;
        }

        public static bool IsPiercing(this ShotModifiers modifiers)
        {
            return (modifiers & ShotModifiers.PiercingShot) != 0;
        }

        public static bool IsAccelerated(this ShotModifiers modifiers)
        {
            return (modifiers & ShotModifiers.AcceleratedShot) != 0;
        }

        public static bool IsCooldownReduced(this ShotModifiers modifiers)
        {
            return (modifiers & ShotModifiers.ReducedCooldown) != 0;
        }
    }
}