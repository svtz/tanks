namespace svtz.Tanks.UserInterface.States
{
    internal sealed class InGameGUIState : AbstractGUIState
    {
        public override GUIState Key
        {
            get { return GUIState.InGame; }
        }

        public override void OnEscape()
        {
            GoToState(GUIState.GameMenu);
        }
    }
}