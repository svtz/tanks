namespace svtz.Tanks.Projectile
{
    internal enum PiercingArmorKind
    {
        /// <summary> Пробивается насквозь </summary>
        NoArmor,

        /// <summary> Насквозь пробивается только заряженным выстрелом </summary>
        PiercingProof,

        /// <summary> Насквозь не пробивается ничем </summary>
        Unpierceable
    }
}