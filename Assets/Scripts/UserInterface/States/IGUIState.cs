namespace svtz.Tanks.UserInterface.States
{
    internal interface IGUIState
    {
        GUIState Key { get; }
        GUIState OnGUI();
        GUIState OnEscapePressed();
    }
}