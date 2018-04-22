using UnityEngine;
using Zenject;

namespace svtz.Tanks.Assets.Scripts.Common
{
    internal abstract class GameObjectWrapper<T>
        where T : GameObjectWrapper<T>
    {
        protected GameObject GameObject { get; private set; }

        protected GameObjectWrapper(DiContainer container, GameObject prefab)
        {
            // ��������, ���� �����-�� ����� ������ ������ �������� ���� � ����������?
            var sub = container.CreateSubContainer();
            sub.Bind(GetType()).FromInstance(this);

            GameObject = sub.InstantiatePrefab(prefab);
        }

        protected GameObjectWrapper(DiContainer container, GameObject prefab, Transform parent)
        {
            // ��������, ���� �����-�� ����� ������ ������ �������� ���� � ����������?
            var sub = container.CreateSubContainer();
            sub.Bind(GetType()).FromInstance(this);

            GameObject = sub.InstantiatePrefab(prefab, parent);
        }
    }
}