using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace svtz.Tanks.UserInterface
{
    public class SelectFirstIfSelectionEmpty : MonoBehaviour
    {
        private EventSystem _eventSystem;

        [Inject]
        private void Construct(EventSystem eventSystem)
        {
            _eventSystem = eventSystem;
        }

        private void Start()
        {
            var selectable = GetComponent<Selectable>();
            if (selectable != null && selectable.interactable)
            {
                var menuNavigator = GetComponentInParent<MenuNavigator>();
                if (menuNavigator != null && menuNavigator.FirstSelectedGameObject == null)
                {
                    menuNavigator.FirstSelectedGameObject = gameObject;
                    _eventSystem.SetSelectedGameObject(gameObject);
                }
            }
            Destroy(this);
        }
    }
}
