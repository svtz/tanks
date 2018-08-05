using System;

namespace svtz.Tanks.Projectile
{
    internal enum ShotArmorKind
    {
        /// <summary> Убивается любым выстрелом </summary>
        NoArmor = 0,

        /// <summary> Убивается только заряженным выстрелом </summary>
        ShotProof,

        /// <summary> Ничем не убивается </summary>
        Unbreakable
    }
}