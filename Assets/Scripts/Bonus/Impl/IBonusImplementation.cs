using UnityEngine;

namespace svtz.Tanks.Bonus.Impl
{
    internal interface IBonusImplementation
    {
        BonusKind BonusKind { get; }
        void Apply(GameObject player);
    }
}