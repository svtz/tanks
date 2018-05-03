namespace svtz.Tanks.UserInterface.States
{
    internal sealed class ClientLobbyGUIState : LobbyGUIState
    {
        public override GUIState Key
        {
            get { return GUIState.ClientLobby; }
        }

        public override void OnEscape()
        {
            base.OnEscape();
            GoToState(GUIState.StartClient);
        }
    }
}