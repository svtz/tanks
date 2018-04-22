using UnityEngine;
using Zenject;

namespace svtz.Tanks.Assets.Scripts.Common
{
    internal abstract class GameObjectWrapper<T>
        where T : GameObjectWrapper<T>
    {
        public GameObject GameObject { get; private set; }

        protected GameObjectWrapper(IInstantiator container, GameObject prefab)
        {
            GameObject = container.InstantiatePrefab(prefab);
        }

        protected GameObjectWrapper(IInstantiator container, GameObject prefab, Transform parent)
        {
            GameObject = container.InstantiatePrefab(prefab, parent);
        }
    }
}