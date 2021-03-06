﻿using System.Linq;
using svtz.Tanks.BattleStats;
using svtz.Tanks.Common;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using Zenject;

namespace svtz.Tanks.Network
{
    internal sealed class CustomNetworkManager : NetworkLobbyManager
    {
        private CustomNetworkDiscovery _networkDiscovery;
        private TeamManager _teamManager;
        private ConnectedToServerSignal _connectedToServerSignal;
        private DisconnectedFromServerSignal _disconnectedFromServerSignal;
        private GameStartedSignal _gameStartedSignal;
        private BattleStatsManager _battleStatsManager;

        [Inject]
        public void Construct(TeamManager teamManager,
            CustomNetworkDiscovery networkDiscovery,
            ConnectedToServerSignal connectedToServerSignal,
            DisconnectedFromServerSignal disconnectedFromServerSignal,
            GameStartedSignal gameStartedSignal,
            BattleStatsManager battleStatsManager)
        {
            _teamManager = teamManager;
            _connectedToServerSignal = connectedToServerSignal;
            _disconnectedFromServerSignal = disconnectedFromServerSignal;
            _networkDiscovery = networkDiscovery;
            _gameStartedSignal = gameStartedSignal;
            _battleStatsManager = battleStatsManager;
        }

        public override void OnLobbyServerPlayersReady()
        {
            _networkDiscovery.StopBroadcast();

            var names = lobbySlots.OfType<CustomLobbyPlayer>()
                .ToDictionary(p => p.connectionToClient.connectionId, p => p.PlayerName);

            base.OnLobbyServerPlayersReady();

            _battleStatsManager.ServerGameStarted(names);
        }

        public override GameObject OnLobbyServerCreateGamePlayer(NetworkConnection conn, short playerControllerId)
        {
            var player = Instantiate(gamePlayerPrefab, Vector3.zero, Quaternion.identity);
            NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
            return player;
        }

        public override void OnLobbyServerConnect(NetworkConnection conn)
        {
            base.OnLobbyServerConnect(conn);

            _teamManager.RegisterPlayer(conn);
        }

        public override void OnStartClient(NetworkClient lobbyClient)
        {
            base.OnStartClient(lobbyClient);

            _connectedToServerSignal.Fire(lobbyClient);
        }

        public override void OnLobbyClientSceneChanged(NetworkConnection conn)
        {
            base.OnLobbyClientSceneChanged(conn);
            if (SceneManager.GetSceneAt(0).name == playScene)
                _gameStartedSignal.Fire();
        }

        public override void OnClientDisconnect(NetworkConnection conn)
        {
            base.OnClientDisconnect(conn);
            _disconnectedFromServerSignal.Fire();
    }
}
}
