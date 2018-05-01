using svtz.Tanks.Common;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using Zenject;

namespace svtz.Tanks.Network
{
    public class CustomNetworkManager : NetworkLobbyManager
    {
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
            GetComponent<CustomNetworkDiscovery>().StopBroadcast();
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

            // фу-фу-фу таким быть, но пока Network не запаковано в контейнер, будет так
            var teamManager = ProjectContext.Instance.Container.Resolve<TeamManager>();
            teamManager.RegisterPlayer(conn);
        }
        
        public override void OnLobbyClientSceneChanged(NetworkConnection conn)
        {
            base.OnLobbyClientSceneChanged(conn);
            if (SceneManager.GetSceneAt(0).name == playScene)
                GetComponent<CustomNetworkManagerHUD>().CloseMenu();
        }

        public override void OnClientDisconnect(NetworkConnection conn)
        {
            base.OnClientDisconnect(conn);
            GetComponent<CustomNetworkManagerHUD>().ShowMenu(GUIState.MainMenu);
        }
    }
}
