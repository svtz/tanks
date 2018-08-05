using svtz.Tanks.Projectile;
using UnityEngine;

namespace svtz.Tanks.Tank
{
    internal interface IGun
    {
        bool CanFire { get; }
        void Fire(Transform start, GameObject owner, ShotModifiers additionalModifiers);

        void Reload();
    }
}