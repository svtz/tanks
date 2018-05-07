using System.Linq;
using svtz.Tanks.Network;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace svtz.Tanks.UserInterface.States
{
    internal sealed class StartClientDirectIPGUIState : StartClientGUIStateBase, IInitializable
    {
#pragma warning disable 0649
        public RectTransform IpAddressInput;
        public RectTransform PortInput;
        public RectTransform ConnectButton;
        public Color ErrorColor;
#pragma warning restore 0649

        private Color _normalColor;
        private Text _portText;
        private Text _ipText;
        private Button _connectButton;

        private ServerData _serverData = new ServerData();

        public override GUIState Key
        {
            get { return GUIState.StartClientDirectIP; }
        }

        void IInitializable.Initialize()
        {
            _connectButton = ConnectButton.GetComponent<Button>();
            _portText = PortInput.Find("Text").GetComponent<Text>();
            _ipText = IpAddressInput.Find("Text").GetComponent<Text>();
            _normalColor = _portText.color;
        }

        public override void OnEnterState()
        {
            base.OnEnterState();

            PlayerNameInput.GetComponent<InputField>().text = NetworkDiscovery.PlayerName;
            IpAddressInput.GetComponent<InputField>().text = "127.0.0.1";
            PortInput.GetComponent<InputField>().text = "7777";
        }

        public override void OnReturn()
        {
            base.OnReturn();

            GoToState(GUIState.MainMenu);
        }

        private bool _ipValid = true;
        private bool _portValid = true;

        public void SetIpAddress(string newAddressString)
        {
            if (!ValidateIPv4(newAddressString))
            {
                _ipText.color = ErrorColor;
                _connectButton.interactable = false;
                _ipValid = false;
            }
            else
            {
                _serverData = ServerData.Create("", newAddressString, _serverData.Port);

                _ipText.color = _normalColor;
                _connectButton.interactable = _portValid;
                _ipValid = true;
            }
        }

        private bool ValidateIPv4(string ipString)
        {
            if (string.IsNullOrEmpty(ipString) || ipString.Trim() == string.Empty)
            {
                return false;
            }

            var splitValues = ipString.Split('.');
            if (splitValues.Length != 4)
            {
                return false;
            }

            byte tempForParsing;
            return splitValues.All(r => byte.TryParse(r, out tempForParsing));
        }

        public void SetPort(string newPortString)
        {
            int newValue;
            if (!int.TryParse(newPortString, out newValue) || newValue < 1 || newValue > 65535)
            {
                _portText.color = ErrorColor;
                _connectButton.interactable = false;
                _portValid = false;
            }
            else
            {
                _serverData = ServerData.Create("", _serverData.NetworkAddress, newValue);

                _portText.color = _normalColor;
                _connectButton.interactable = _ipValid;
                _portValid = true;
            }
        }

        public void DoConnect()
        {
            ConnectToServer(_serverData);
        }
    }
}