using System.Collections;
using System.Collections.Generic;
using svtz.Tanks.Network;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace svtz.Tanks.UserInterface.States
{
    internal abstract class LobbyGUIState : NetworkMenuGUIState
    {
        private CustomNetworkManager NetworkManager { get; set; }
        private LobbyGUISettings _settings;

        private readonly Dictionary<CustomLobbyPlayer, RectTransform> _playerItems = 
            new Dictionary<CustomLobbyPlayer, RectTransform>();

        [Inject]
        private void Construct(CustomNetworkManager networkManager, LobbyGUISettings settings)
        {
            NetworkManager = networkManager;
            _settings = settings;
        }

        public override void OnReturn()
        {
            base.OnReturn();
            NetworkDiscovery.CustomStop();
        }

        public override void OnEnterState()
        {
            base.OnEnterState();

            StartCoroutine(RefreshPlayers());
        }


        public override void OnExitState()
        {
            base.OnExitState();

            StopCoroutine(RefreshPlayers());
        }

        private IEnumerator RefreshPlayers()
        {
            while (true)
            {

                var outdatedPlayers = new HashSet<CustomLobbyPlayer>(_playerItems.Keys);

                if (NetworkManager != null)
                {
                    foreach (var player in NetworkManager.lobbySlots)
                    {
                        var customLobbyPlayer = player as CustomLobbyPlayer;
                        if (customLobbyPlayer == null)
                            continue;

                        RectTransform item;
                        if (!_playerItems.TryGetValue(customLobbyPlayer, out item))
                        {
                            item = Instantiate(_settings.PlayerItemPrefab);
                            item.SetParent(_settings.ScrollContent, false);
                            _playerItems.Add(customLobbyPlayer, item);
                        }
                        else
                        {
                            outdatedPlayers.Remove(customLobbyPlayer);
                        }

                        var toggle = item.GetComponentInChildren<Toggle>();
                        toggle.interactable = customLobbyPlayer.isLocalPlayer;
                        if (customLobbyPlayer.isLocalPlayer)
                        {
                            customLobbyPlayer.SetReady(toggle.isOn);
                        }
                        else
                        {
                            toggle.isOn = customLobbyPlayer.readyToBegin;
                        }

                        var text = item.GetComponentInChildren<Text>();
                        text.text = customLobbyPlayer.PlayerName;
                    }
                }

                foreach (var player in outdatedPlayers)
                {
                    Destroy(_playerItems[player].gameObject);
                    _playerItems.Remove(player);
                }

                yield return new WaitForSeconds(_settings.RefreshIntervalSeconds);
            }
        }
    }
}