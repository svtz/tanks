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
        private GameStartedSignal _gameStartedSignal;

        [Inject]
        public void Construct(TeamManager teamManager,
            CustomNetworkDiscovery networkDiscovery,
            ConnectedToServerSignal connectedToServerSignal,
            GameStartedSignal gameStartedSignal)
        {
            _teamManager = teamManager;
            _connectedToServerSignal = connectedToServerSignal;
            _networkDiscovery = networkDiscovery;
            _gameStartedSignal = gameStartedSignal;
        }

        public override void OnLobbyServerPlayersReady()
        {
            _networkDiscovery.StopBroadcast();
            base.OnLobbyServerPlayersReady();
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
    }
}
