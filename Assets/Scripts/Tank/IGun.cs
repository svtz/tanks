using UnityEngine;

namespace svtz.Tanks.Tank
{
    internal interface IGun
    {
        bool CanFire { get; }
        void Fire(Transform start, GameObject owner, int boostLevel);

        void Reload();
    }
}