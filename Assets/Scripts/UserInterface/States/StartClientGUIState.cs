using System;
using System.Collections.Generic;
using svtz.Tanks.Network;
using UnityEngine;
using UnityEngine.UI;

namespace svtz.Tanks.UserInterface.States
{
    internal sealed class StartClientGUIState : StartClientGUIStateBase
    {
#pragma warning disable 0649
        public RectTransform ServerItemPrefab;
        public RectTransform ScrollContent;
#pragma warning restore 0649

        private readonly Dictionary<string, RectTransform> _serverItems = new Dictionary<string, RectTransform>();
        private readonly Dictionary<string, ServerData> _serverData = new Dictionary<string, ServerData>();

        public override void OnEnterState()
        {
            base.OnEnterState();

            NetworkDiscovery.CustomStartServerDiscovery(AddOrReplaceServerItem);
        }

        private void AddOrReplaceServerItem(ServerData data)
        {
            RectTransform item;
            if (!_serverItems.TryGetValue(data.Key, out item))
            {
                item = Instantiate(ServerItemPrefab);
                item.transform.SetParent(ScrollContent, false);

                var button = item.GetComponent<Button>();
                button.onClick.AddListener(() => ConnectToServer(_serverData[data.Key]));

                _serverItems.Add(data.Key, item);
            }

            var text = item.GetComponentInChildren<Text>();
            var serverTitle = string.Concat(data.ServerName, Environment.NewLine, data.NetworkAddress, ":", data.Port);
            text.text = serverTitle;
            _serverData[data.Key] = data;
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

        public override void OnReturn()
        {
            NetworkDiscovery.CustomStopServerDiscovery();
            base.OnReturn();
        }

        public override GUIState Key
        {
            get { return GUIState.StartClient; }
        }
    }
}