using UnityEngine.EventSystems;
using Zenject;

namespace svtz.Tanks.UserInterface
{
    public sealed class MouseOverSelector : EventTrigger
    {
        private EventSystem _eventSystem;

        [Inject]
        private void Construct(EventSystem eventSystem)
        {
            _eventSystem = eventSystem;
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            _eventSystem.SetSelectedGameObject(gameObject);
        }
    }
}

