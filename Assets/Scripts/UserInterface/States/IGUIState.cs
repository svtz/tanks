namespace svtz.Tanks.UserInterface.States
{
    internal interface IGUIState
    {
        GUIState Key { get; }

        void OnExitState();
        void OnEnterState();

        void OnReturn();
    }
}