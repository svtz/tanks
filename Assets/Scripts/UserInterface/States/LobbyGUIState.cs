using svtz.Tanks.Network;
using UnityEngine;

namespace svtz.Tanks.UserInterface.States
{
    internal abstract class LobbyGUIState : NetworkMenuGUIState
    {
        private CustomNetworkManager NetworkManager { get; set; }

        protected LobbyGUIState(GUISkin guiSkin, CustomNetworkDiscovery networkDiscovery,
            CustomNetworkManager networkManager) : base(guiSkin, networkDiscovery)
        {
            NetworkManager = networkManager;
        }

        private void DrawPlayerGUI(CustomLobbyPlayer player)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(player.PlayerName, GetStyle("LobbyPlayerLabel"));
            if (player.isLocalPlayer)
            {
                if (GUILayout.Button(player.readyToBegin ? "ГОТОВ" : "Не готов", GetStyle("ReadyButton")))
                {
                    player.ToggleReady();
                }
            }
            else
            {
                GUILayout.Label(player.readyToBegin ? "ГОТОВ" : "Не готов", GetStyle("ReadyButtonDisabled"));
            }
            GUILayout.EndHorizontal();
        }

        public sealed override GUIState OnGUI()
        {
            var nextState = Key;

            CenterScreen(() =>
            {
                GUILayout.Label("ОЖИДАНИЕ ИГРОКОВ", GetStyle("MenuTitle"));

                foreach (var player in NetworkManager.lobbySlots)
                {
                    var customLobbyPlayer = player as CustomLobbyPlayer;
                    if (customLobbyPlayer != null)
                    {
                        DrawPlayerGUI(customLobbyPlayer);
                    }
                    else
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();

                        GUILayout.Label("—", GetStyle("LobbyPlayerStub"));

                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();
                    }
                }

                if (GUILayout.Button("Вернуться назад", GetStyle("ReturnButton")))
                {
                    nextState = OnEscapePressed();
                }
            });

            return nextState;
        }
    }
}