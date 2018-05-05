using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace svtz.Tanks.UserInterface.States
{
    internal abstract class AbstractGUIState : MonoBehaviour, IGUIState
    {
        private GUIManager _guiManager;

        public abstract GUIState Key { get; }

        public virtual void OnExitState()
        {
            gameObject.SetActive(false);
        }

        public virtual void OnEnterState()
        {
            FindObjectOfType<EventSystem>().SetSelectedGameObject(null);
            gameObject.SetActive(true);
        }

        protected virtual void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                OnEscape();
        }

        protected virtual void OnEscape()
        {
        }

        [Inject]
        public void Construct(GUIManager guiManager)
        {
            _guiManager = guiManager;
        }

        protected void GoToState(GUIState newState)
        {
            _guiManager.GoToState(newState);
        }
    }
}