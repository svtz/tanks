using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace svtz.Tanks.UserInterface.States
{
    internal sealed class StartServerGUIState : NetworkMenuGUIState, IInitializable
    {
#pragma warning disable 0649
        public RectTransform ServerNameInput;
        public RectTransform PlayerNameInput;
        public RectTransform PortInput;
        public RectTransform CreateButton;
        public Color ErrorColor;
#pragma warning restore 0649

        private Color _normalColor;
        private Text _portText;
        private Button _createButton;

        public override GUIState Key
        {
            get { return GUIState.StartServer; }
        }

        void IInitializable.Initialize()
        {
            _createButton = CreateButton.GetComponent<Button>();
            _portText = PortInput.Find("Text").GetComponent<Text>();
            _normalColor = _portText.color;
        }

        public override void OnEnterState()
        {
            base.OnEnterState();

            ServerNameInput.GetComponent<InputField>().text = NetworkDiscovery.ServerName;
            PortInput.GetComponent<InputField>().text = NetworkDiscovery.NetworkPort.ToString();
            PlayerNameInput.GetComponent<InputField>().text = NetworkDiscovery.PlayerName;
            _portText.color = _normalColor;
        }

        public void SetServerName(string newName)
        {
            NetworkDiscovery.ServerName = newName;
        }

        public void SetPort(string newPortString)
        {
            int newValue;
            if (!int.TryParse(newPortString, out newValue) || newValue < 1 || newValue > 65535)
            {
                _portText.color = ErrorColor;
                _createButton.interactable = false;
            }
            else
            {
                NetworkDiscovery.NetworkPort = newValue;
                _portText.color = _normalColor;
                _createButton.interactable = true;
            }
        }

        public void SetPlayerName(string newName)
        {
            NetworkDiscovery.PlayerName = newName;
        }

        public void DoCreate()
        {
            if (NetworkDiscovery.CustomStartServer())
                GoToState(GUIState.ServerLobby);
        }

        public override void OnEscape()
        {
            GoToState(GUIState.MainMenu);
        }
    }
}