using UnityEngine;
using Zenject;

namespace svtz.Tanks.Common
{
    internal abstract class GameObjectHolder<T>
        where T : GameObjectHolder<T>
    {
        protected GameObject GameObject { get; private set; }

        protected abstract GameObject Prefab { get; }

        protected virtual GameObjectCreationParameters CreationParameters
        {
            get { return GameObjectCreationParameters.Default; }
        }

        /// <summary> Создание объекта оформлено в виде отдельного метода из эстетических соображений
        /// (тащить через все конструкторы, причём в виде именно контейнера  (а не IInstantiator) - не комильфо) </summary>
        [Inject]
        private void Construct(DiContainer container)
        {
            // возможно, есть какой-то более лучший способ передать себя в компоненты?
            var sub = container.CreateSubContainer();
            sub.Bind(GetType()).FromInstance(this);

            GameObject = sub.InstantiatePrefab(Prefab, CreationParameters);

            OnCreated();
        }

        protected virtual void OnCreated()
        {
        }
    }
}