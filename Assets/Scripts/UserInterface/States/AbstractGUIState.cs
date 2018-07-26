using svtz.Tanks.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace svtz.Tanks.UserInterface.States
{
    internal abstract class AbstractGUIState : MonoBehaviour, IGUIState
    {
        private GUIManager _guiManager;
        protected InputManager Input { get; private set; }

        public abstract GUIState Key { get; }

        public virtual void OnExitState()
        {
            gameObject.SetActive(false);
        }

        public virtual void OnEnterState()
        {
            gameObject.SetActive(true);
        }

        protected virtual void Update()
        {
            if (Input.Return())
                OnReturn();
        }

        public virtual void OnReturn()
        {
        }

        [Inject]
        public void Construct(GUIManager guiManager, InputManager input, EventSystem eventSystem)
        {
            _guiManager = guiManager;
            Input = input;
        }

        protected void GoToState(GUIState newState)
        {
            _guiManager.GoToState(newState);
        }
    }
}