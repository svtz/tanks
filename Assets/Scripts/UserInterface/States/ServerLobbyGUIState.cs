namespace svtz.Tanks.UserInterface.States
{
    internal sealed class ServerLobbyGUIState : LobbyGUIState
    {
        public override GUIState Key
        {
            get { return GUIState.ServerLobby; }
        }

        public override void OnReturn()
        {
            base.OnReturn();
            GoToState(GUIState.StartServer);
        }
    }
}