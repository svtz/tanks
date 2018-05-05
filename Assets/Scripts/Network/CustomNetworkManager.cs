using System.Linq;
using svtz.Tanks.BattleStats;
using svtz.Tanks.Common;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;

namespace svtz.Tanks.Network
{
    internal sealed class CustomNetworkManager : NetworkLobbyManager
    {
        private CustomNetworkDiscovery _networkDiscovery;
        private TeamManager _teamManager;
        private ConnectedToServerSignal _connectedToServerSignal;
        private DisconnectedFromServerSignal _disconnectedFromServerSignal;
        private GameStartedSignal.ServerToClient _gameStartedSignal;

        [Inject]
        public void Construct(TeamManager teamManager,
            CustomNetworkDiscovery networkDiscovery,
            ConnectedToServerSignal connectedToServerSignal,
            DisconnectedFromServerSignal disconnectedFromServerSignal,
            GameStartedSignal.ServerToClient gameStartedSignal)
        {
            _teamManager = teamManager;
            _connectedToServerSignal = connectedToServerSignal;
            _disconnectedFromServerSignal = disconnectedFromServerSignal;
            _networkDiscovery = networkDiscovery;
            _gameStartedSignal = gameStartedSignal;
        }

        public override void OnLobbyServerPlayersReady()
        {
            _networkDiscovery.StopBroadcast();

            var namesByNetId = lobbySlots.OfType<CustomLobbyPlayer>().ToDictionary(p => p.netId, p => p.PlayerName);
            var namesByConId = NetworkServer.connections
                .ToDictionary(
                    con => con.connectionId,
                    con => namesByNetId[con.clientOwnedObjects.Single()]);

            base.OnLobbyServerPlayersReady();

            _gameStartedSignal.FireOnAllClients(new GameStartedSignal.Msg { PlayerNames = namesByConId });
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

        public override void OnClientDisconnect(NetworkConnection conn)
        {
            base.OnClientDisconnect(conn);
            _disconnectedFromServerSignal.Fire();
    }
}
}
