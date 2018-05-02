using System;
using System.Linq;
using svtz.Tanks.Network;
using UnityEngine;

namespace svtz.Tanks.UserInterface.States
{
    internal sealed class StartClientGUIState : NetworkMenuGUIState
    {
        private Vector2 _scrollPosition = new Vector2(0, 0);

        public StartClientGUIState(GUISkin guiSkin, CustomNetworkDiscovery networkDiscovery) : base(guiSkin, networkDiscovery)
        {
        }

        public override GUIState Key
        {
            get { return GUIState.StartClient; }
        }

        public override GUIState OnGUI()
        {
            var nextState = Key;

            CenterScreen(() =>
            {
                MenuTitle("ПОИСК ИГРЫ");

                GUILayout.Label("Имя игрока:");
                NetworkDiscovery.playerName = GUILayout.TextField(NetworkDiscovery.playerName);

                GUILayout.Label("Найденные игры:");

                if (NetworkDiscovery.foundServers.Any())
                {
                    _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);
                    foreach (var record in NetworkDiscovery.foundServers)
                    {
                        var serverTitle = string.Concat(record.ServerName, Environment.NewLine, record.NetworkAddress, ":", record.Port);
                        if (GUILayout.Button(serverTitle, GetStyle("ServerButton")))
                        {
                            NetworkDiscovery.CustomStartClient(record);
                            nextState = GUIState.ClientLobby;
                        }
                    }
                    GUILayout.EndScrollView();
                }
                else
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();

                    GUILayout.Label("<идёт поиск>");

                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                }

                if (ReturnButton("НАЗАД"))
                {
                    nextState = OnEscapePressed();
                }
            });
            return nextState;
        }

        public override GUIState OnEscapePressed()
        {
            NetworkDiscovery.CustomStopServerDiscovery();
            return GUIState.MainMenu;
        }
    }
}