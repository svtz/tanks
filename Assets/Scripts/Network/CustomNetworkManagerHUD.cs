using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public enum GUIState
{
    MainMenu, StartServer, StartClient, ServerLobby, ClientLobby, InGame, GameMenu
}

[RequireComponent(typeof(CustomNetworkDiscovery))]
public class CustomNetworkManagerHUD : MonoBehaviour {
    public GUISkin skin;
    private GUIState state = 0;
    public bool showGUI = true;
    public string gameName = "ПанкоТанки";
    private Vector2 scrollPosition = new Vector2(0, 0);

    private Rect GamePanel()
    {
        GUIStyle style = skin.GetStyle("MenuArea");
        return new Rect(
            (Screen.width - style.fixedWidth) / 2,
            (Screen.height - style.fixedHeight) / 2,
            style.fixedWidth,
            style.fixedHeight);
    }

    private Rect LobbyPanel()
    {
        GUIStyle style = skin.GetStyle("LobbyArea");
        return new Rect(
            style.padding.left,
            style.padding.top,
            style.fixedWidth,
            Screen.height - style.padding.top - style.padding.bottom);
    }

    public void CloseMenu()
    {
        state = GUIState.InGame;
    }

    public void ShowMenu()
    {
        state = GUIState.GameMenu;
    }

    private void EscapeHandler(bool escape)
    {
        if (!escape)
            return;
        switch (state)
        {
            case GUIState.StartServer:
                state = GUIState.MainMenu;
                break;
            case GUIState.StartClient:
                GetComponent<CustomNetworkDiscovery>().CustomStopServerDiscovery();
                state = GUIState.MainMenu;
                break;
            case GUIState.ServerLobby:
                GetComponent<CustomNetworkDiscovery>().CustomStop();
                state = GUIState.StartServer;
                break;
            case GUIState.ClientLobby:
                GetComponent<CustomNetworkDiscovery>().CustomStop();
                GetComponent<CustomNetworkDiscovery>().CustomStartServerDiscovery();
                state = GUIState.StartClient;
                break;
            case GUIState.InGame:
                state = GUIState.GameMenu;
                break;
            case GUIState.GameMenu:
                state = GUIState.InGame;
                break;
            default:
                break;
        }


    }

    private void OnGUI()
    {
        if (showGUI)
        {
            if (skin != null)
                GUI.skin = skin;
            //GetComponent<CustomNetworkDiscovery>().
            switch (state)
            {
                case GUIState.MainMenu:
                    GUI.Box(GamePanel(), "");
                    GUILayout.BeginArea(GamePanel(), skin.GetStyle("MenuArea"));
                    GUILayout.BeginVertical();
                    GUILayout.Label(gameName);
                    if (GUILayout.Button("Присоединиться к игре"))
                    {
                        GetComponent<CustomNetworkDiscovery>().CustomStartServerDiscovery();
                        state = GUIState.StartClient;
                    }
                    if (GUILayout.Button("Создать игру"))
                        state = GUIState.StartServer;
                    if (GUILayout.Button("Покинуть игру"))
                        Application.Quit();
                    GUILayout.EndVertical();
                    GUILayout.EndArea();
                        break;
                case GUIState.StartServer:
                    GUI.Box(GamePanel(), "");
                    GUILayout.BeginArea(GamePanel(), skin.GetStyle("MenuArea"));
                    GUILayout.BeginVertical();
                    GUILayout.Label("Введите название игры");
                    GetComponent<CustomNetworkDiscovery>().serverName =
                        GUILayout.TextField(GetComponent<CustomNetworkDiscovery>().serverName);
                    GUILayout.Label("Введите порт");
                    string newPort = GUILayout.TextField(GetComponent<CustomNetworkDiscovery>().networkPort.ToString());
                    int i_new_port = 0;
                    if (int.TryParse(newPort, out i_new_port))
                        GetComponent<CustomNetworkDiscovery>().networkPort = i_new_port;
                    GUILayout.Label("Введите имя");
                    GetComponent<CustomNetworkDiscovery>().playerName =
                        GUILayout.TextField(GetComponent<CustomNetworkDiscovery>().playerName);
                    if (GUILayout.Button("Создать игру"))
                    {
                        if (GetComponent<CustomNetworkDiscovery>().CustomStartServer())
                            state = GUIState.ServerLobby;
                    }
                    EscapeHandler(GUILayout.Button("Вернуться назад"));
                    GUILayout.EndVertical();
                    GUILayout.EndArea();
                    break;
                case GUIState.StartClient:
                    GUI.Box(GamePanel(), "");
                    GUILayout.BeginArea(GamePanel(), skin.GetStyle("MenuArea"));
                    GUILayout.BeginVertical();
                    GUILayout.Label("Введите имя");
                    GetComponent<CustomNetworkDiscovery>().playerName =
                        GUILayout.TextField(GetComponent<CustomNetworkDiscovery>().playerName);
                    GUILayout.Label("Выберите игру");
                    scrollPosition = GUILayout.BeginScrollView(scrollPosition);
                    foreach(ServerData record in GetComponent<CustomNetworkDiscovery>().findedServers)
                    {
                        if (GUILayout.Button(record.ServerName +  "("+ record.NetworkAddress+":"+record.Port +")"))
                        {
                            GetComponent<CustomNetworkDiscovery>().CustomStartClient(record);
                            state = GUIState.ClientLobby;
                        }
                    }
                    GUILayout.EndScrollView();


                    EscapeHandler(GUILayout.Button("Вернуться назад"));
                    GUILayout.EndVertical();
                    GUILayout.EndArea();
                    break;
                case GUIState.ServerLobby:
                    GUI.Box(LobbyPanel(), "");
                    GUILayout.BeginArea(LobbyPanel(), skin.GetStyle("LobbyArea"));
                    GUILayout.BeginVertical();
                    foreach (NetworkLobbyPlayer player in GetComponent<NetworkLobbyManager>().lobbySlots)
                    {
                        if (player != null)
                            ((CustomLobbyPlayer)player).DrawGUI();
                    }
                    GUILayout.FlexibleSpace();
                    EscapeHandler(GUILayout.Button("Вернуться назад"));
                    GUILayout.EndVertical();
                    GUILayout.EndArea();
                    break;
                case GUIState.ClientLobby:
                    GUI.Box(LobbyPanel(), "");
                    GUILayout.BeginArea(LobbyPanel(), skin.GetStyle("LobbyArea"));
                    GUILayout.BeginVertical();
                    foreach (NetworkLobbyPlayer player in GetComponent<NetworkLobbyManager>().lobbySlots)
                    {
                        if (player != null)
                            ((CustomLobbyPlayer)player).DrawGUI();
                    }
                    GUILayout.FlexibleSpace();
                    EscapeHandler(GUILayout.Button("Вернуться назад"));
                    GUILayout.EndVertical();
                    GUILayout.EndArea();
                    break;
                case GUIState.InGame:
                    break;
                case GUIState.GameMenu:
                    GUI.Box(GamePanel(), "");
                    GUILayout.BeginArea(GamePanel(), skin.GetStyle("MenuArea"));
                    GUILayout.BeginVertical();
                    GUILayout.Label(gameName);
                    GUILayout.Label("Игровое меню");

                    EscapeHandler(GUILayout.Button("Вернуться в игру"));
                    if (GUILayout.Button("Покинуть игру"))
                    {
                        GetComponent<CustomNetworkDiscovery>().CustomStop();
                        state = GUIState.MainMenu;
                    }
                    GUILayout.EndVertical();
                    GUILayout.EndArea();
                    break;
                default:
                    break;
            }
        }
    }

    private void Update()
    {
        EscapeHandler(Input.GetKeyDown(KeyCode.Escape));
    }

}
