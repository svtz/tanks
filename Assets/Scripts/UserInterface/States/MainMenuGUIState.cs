#if !UNITY_EDITOR
using UnityEngine;
#endif

namespace svtz.Tanks.UserInterface.States
{
    internal sealed class MainMenuGUIState : AbstractGUIState
    {
        public override GUIState Key
        {
            get { return GUIState.MainMenu; }
        }

        public void CreateGame()
        {
            GoToState(GUIState.StartServer);
        }

        public void SearchGame()
        {
            GoToState(GUIState.StartClient);
        }

        public void Exit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}