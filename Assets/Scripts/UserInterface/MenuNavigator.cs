using svtz.Tanks.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace svtz.Tanks.UserInterface
{
    public sealed class MenuNavigator : MonoBehaviour
    {
#pragma warning disable 0649
        public GameObject FirstSelectedGameObject;
#pragma warning restore 0649

        private EventSystem _eventSystem;
        private InputManager _inputManager;

        [Inject]
        private void Construct(EventSystem eventSystem, InputManager inputManager)
        {
            _eventSystem = eventSystem;
            _inputManager = inputManager;
        }

        private bool _shouldSelectFirstObject = true;

        private void OnEnable()
        {
            _shouldSelectFirstObject = true;
        }

        private void LateUpdate()
        {
            if (_shouldSelectFirstObject)
            {
                _eventSystem.SetSelectedGameObject(FirstSelectedGameObject);
                _shouldSelectFirstObject = false;
            }
        }

        private void Update()
        {
            if (_inputManager.Tab())
            {
                TabBetweenObjects();
            }
            else if (_inputManager.Fire())
            {
                ClickSelectedObject();
            }
        }

        private void ClickSelectedObject()
        {
            var currentSelectedGameObject = _eventSystem.currentSelectedGameObject;
            if (currentSelectedGameObject != null)
            {
                ExecuteEvents.Execute(currentSelectedGameObject, new PointerEventData(_eventSystem), ExecuteEvents.pointerClickHandler);
            }
        }

        private void TabBetweenObjects()
        {
            var currentSelectedGameObject = _eventSystem.currentSelectedGameObject;
            if (currentSelectedGameObject != null)
            {
                var currentSelectable = currentSelectedGameObject.GetComponent<Selectable>();
                if (currentSelectable != null)
                {
                    var next = currentSelectable.FindSelectableOnDown();
                    if (next != null)
                    {
                        _eventSystem.SetSelectedGameObject(next.gameObject);
                        return;
                    }
                }
            }

            _eventSystem.SetSelectedGameObject(FirstSelectedGameObject);
        }
    }
}