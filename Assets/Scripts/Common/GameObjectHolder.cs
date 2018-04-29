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

        /// <summary> �������� ������� ��������� � ���� ���������� ������ �� ������������ �����������
        /// (������ ����� ��� ������������, ������ � ���� ������ ����������  (� �� IInstantiator) - �� ��������) </summary>
        [Inject]
        private void Construct(DiContainer container)
        {
            // ��������, ���� �����-�� ����� ������ ������ �������� ���� � ����������?
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