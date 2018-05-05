using System;
using System.Collections.Generic;
using svtz.Tanks.Network;
using UnityEngine;
using UnityEngine.UI;

namespace svtz.Tanks.UserInterface.States
{
    internal sealed class StartClientGUIState : NetworkMenuGUIState
    {
#pragma warning disable 0649
        public RectTransform ServerItemPrefab;
        public RectTransform ScrollContent;
        public RectTransform PlayerNameInput;
#pragma warning restore 0649

        private readonly Dictionary<string, RectTransform> _serverItems = new Dictionary<string, RectTransform>();
        private readonly Dictionary<string, ServerData> _serverData = new Dictionary<string, ServerData>();

        public override GUIState Key
        {
            get { return GUIState.StartClient; }
        }

        public override void OnEnterState()
        {
            base.OnEnterState();

            NetworkDiscovery.CustomStartServerDiscovery(AddOrReplaceServerItem);
            PlayerNameInput.GetComponent<InputField>().text = NetworkDiscovery.PlayerName;
        }

        public override void OnExitState()
        {
            base.OnExitState();

            foreach (var item in _serverItems)
            {
                Destroy(item.Value.gameObject);
            }
            _serverItems.Clear();
            _serverData.Clear();
        }

        protected override void OnEscape()
        {
            base.OnEscape();

            NetworkDiscovery.CustomStopServerDiscovery();
            GoToState(GUIState.MainMenu);
        }

        private void AddOrReplaceServerItem(ServerData data)
        {
            RectTransform item;
            if (!_serverItems.TryGetValue(data.Key, out item))
            {
                item = Instantiate(ServerItemPrefab);
                item.transform.SetParent(ScrollContent, false);

                var button = item.GetComponent<Button>();
                button.onClick.AddListener(() => ConnectToServer(data.Key));

                _serverItems.Add(data.Key, item);
            }

            var text = item.GetComponentInChildren<Text>();
            var serverTitle = string.Concat(data.ServerName, Environment.NewLine, data.NetworkAddress, ":", data.Port);
            text.text = serverTitle;
            _serverData[data.Key] = data;
        }

        private void ConnectToServer(string key)
        {
            NetworkDiscovery.CustomStartClient(_serverData[key]);
            GoToState(GUIState.ClientLobby);
        }

        public void SetPlayerName(string newName)
        {
            NetworkDiscovery.PlayerName = newName;
        }
    }
}