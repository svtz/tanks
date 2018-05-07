using svtz.Tanks.Network;
using UnityEngine;
using UnityEngine.UI;

namespace svtz.Tanks.UserInterface.States
{
    internal abstract class StartClientGUIStateBase : NetworkMenuGUIState
    {
#pragma warning disable 0649
        public RectTransform PlayerNameInput;
#pragma warning restore 0649

        public override void OnEnterState()
        {
            base.OnEnterState();

            PlayerNameInput.GetComponent<InputField>().text = NetworkDiscovery.PlayerName;
        }

        public override void OnReturn()
        {
            base.OnReturn();

            GoToState(GUIState.MainMenu);
        }

        protected void ConnectToServer(ServerData data)
        {
            NetworkDiscovery.CustomStartClient(data);
            GoToState(GUIState.ClientLobby);
        }

        public void SetPlayerName(string newName)
        {
            NetworkDiscovery.PlayerName = newName;
        }
    }
}