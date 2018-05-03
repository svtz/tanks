namespace svtz.Tanks.UserInterface.States
{
    internal sealed class GameMenuGUIState : NetworkMenuGUIState
    {
        public override GUIState Key
        {
            get { return GUIState.GameMenu; }
        }

        public void Disconnect()
        {
            NetworkDiscovery.CustomStop();
            GoToState(GUIState.MainMenu);
        }

        public override void OnEscape()
        {
            GoToState(GUIState.InGame);
        }
    }
}