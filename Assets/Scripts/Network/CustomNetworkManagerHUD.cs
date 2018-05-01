using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;

namespace svtz.Tanks.Network
{
    public enum GUIState
    {
        MainMenu, StartServer, StartClient, ServerLobby, ClientLobby, InGame, GameMenu
    }

    internal sealed class CustomNetworkManagerHUD : MonoBehaviour {
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

        private CustomNetworkDiscovery _networkDiscovery;
        private CustomNetworkManager _networkManager;

        [Inject]
        public void Construct(
            CustomNetworkDiscovery networkDiscovery,
            CustomNetworkManager networkManager,
            GUISkin guiSkin)
        {
            _networkDiscovery = networkDiscovery;
            _networkManager = networkManager;
            skin = guiSkin;
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
                    _networkDiscovery.CustomStopServerDiscovery();
                    state = GUIState.MainMenu;
                    break;
                case GUIState.ServerLobby:
                    _networkDiscovery.CustomStop();
                    state = GUIState.StartServer;
                    break;
                case GUIState.ClientLobby:
                    _networkDiscovery.CustomStop();
                    _networkDiscovery.CustomStartServerDiscovery();
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
                //_networkDiscovery.
                switch (state)
                {
                    case GUIState.MainMenu:
                        GUI.Box(GamePanel(), "");
                        GUILayout.BeginArea(GamePanel(), skin.GetStyle("MenuArea"));
                        GUILayout.BeginVertical();
                        GUILayout.Label(gameName);
                        if (GUILayout.Button("Присоединиться к игре"))
                        {
                            _networkDiscovery.CustomStartServerDiscovery();
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
                        _networkDiscovery.serverName =
                            GUILayout.TextField(_networkDiscovery.serverName);
                        GUILayout.Label("Введите порт");
                        string newPort = GUILayout.TextField(_networkDiscovery.networkPort.ToString());
                        int i_new_port = 0;
                        if (int.TryParse(newPort, out i_new_port))
                            _networkDiscovery.networkPort = i_new_port;
                        GUILayout.Label("Введите имя");
                        _networkDiscovery.playerName =
                            GUILayout.TextField(_networkDiscovery.playerName);
                        if (GUILayout.Button("Создать игру"))
                        {
                            if (_networkDiscovery.CustomStartServer())
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
                        _networkDiscovery.playerName =
                            GUILayout.TextField(_networkDiscovery.playerName);
                        GUILayout.Label("Выберите игру");
                        scrollPosition = GUILayout.BeginScrollView(scrollPosition);
                        foreach(ServerData record in _networkDiscovery.findedServers)
                        {
                            if (GUILayout.Button(record.ServerName +  "("+ record.NetworkAddress+":"+record.Port +")"))
                            {
                                _networkDiscovery.CustomStartClient(record);
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
                        foreach (NetworkLobbyPlayer player in _networkManager.lobbySlots)
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
                        foreach (NetworkLobbyPlayer player in _networkManager.lobbySlots)
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
                            _networkDiscovery.CustomStop();
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
}