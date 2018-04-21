
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
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
    /*
    public override void ServerChangeScene(string sceneName)
    {
        if (sceneName == lobbyScene)
        {
            foreach (var lobbyPlayer in lobbySlots)
            {
                if (lobbyPlayer == null)
                    continue;

                // find the game-player object for this connection, and destroy it
                var uv = lobbyPlayer.GetComponent<NetworkIdentity>();

                PlayerController playerController;
                if (uv.connectionToClient.GetPlayerController(uv.playerControllerId, out playerController))
                {
                    NetworkServer.Destroy(playerController.gameObject);
                }

                if (NetworkServer.active)
                {
                    // re-add the lobby object
                    lobbyPlayer.GetComponent<NetworkLobbyPlayer>().readyToBegin = false;
                    NetworkServer.ReplacePlayerForConnection(uv.connectionToClient, lobbyPlayer.gameObject, uv.playerControllerId);
                }
            }
        }
        base.ServerChangeScene(sceneName);
    }*/

    public override void OnLobbyClientSceneChanged(NetworkConnection conn)
    {
        base.OnLobbyClientSceneChanged(conn);
        if (SceneManager.GetSceneAt(0).name == playScene)
            GetComponent<CustomNetworkManagerHUD>().CloseMenu();
    }
}
