using svtz.Tanks.Common;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using Zenject;

namespace svtz.Tanks.Network
{
    internal sealed class CustomNetworkManager : NetworkLobbyManager
    {
        private CustomNetworkManagerHUD _hud;
        private CustomNetworkDiscovery _networkDiscovery;
        private TeamManager _teamManager;
        private ConnectedToServerSignal _connectedToServer;

        [Inject]
        public void Construct(CustomNetworkManagerHUD hud,
            TeamManager teamManager,
            CustomNetworkDiscovery networkDiscovery,
            ConnectedToServerSignal connectedToServer)
        {
            _hud = hud;
            _teamManager = teamManager;
            _connectedToServer = connectedToServer;
            _networkDiscovery = networkDiscovery;
        }

        public override NetworkClient StartHost()
        {
            return base.StartHost();
        }
    
        public override bool OnLobbyServerSceneLoadedForPlayer(GameObject lobbyPlayer, GameObject gamePlayer)
        {
            Debug.Log("Loaded");
            return base.OnLobbyServerSceneLoadedForPlayer(lobbyPlayer, gamePlayer);
        }

        public override void OnLobbyServerPlayersReady()
        {
            _networkDiscovery.StopBroadcast();
            base.OnLobbyServerPlayersReady();
        }
        public override GameObject OnLobbyServerCreateGamePlayer(NetworkConnection conn, short playerControllerId)
        {
            Debug.Log("Instantiated");
            GameObject player = (GameObject)Instantiate(gamePlayerPrefab, Vector3.zero, Quaternion.identity);
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

            _connectedToServer.Fire(lobbyClient);
        }

        public override void OnLobbyClientSceneChanged(NetworkConnection conn)
        {
            base.OnLobbyClientSceneChanged(conn);
            if (SceneManager.GetSceneAt(0).name == playScene)
                _hud.CloseMenu();
        }
    }
}
