﻿using System;
using svtz.Tanks.UserInterface.States;
using UnityEngine;
using Zenject;

namespace svtz.Tanks.UserInterface
{
    internal sealed class GUIInstaller : Installer<GUIInstaller.GUIMenus, GUIInstaller>
    {
        [Serializable]
        public class GUIMenus
        {
#pragma warning disable 0649
            public GameObject GuiManagerPrefab;

            public GameObject MainMenu;
            public GameObject SearchGamesMenu;
            public GameObject CreateGameMenu;
            public GameObject LobbyMenu;
            public GameObject GameMenu;
            public GameObject InGameHUD;
            public GameObject ConnectDirectIpMenu;
#pragma warning restore 0649
        }

        private readonly GUIMenus _menus;

        public GUIInstaller(GUIMenus menus)
        {
            _menus = menus;
        }

        public override void InstallBindings()
        {
            // состояния
            Container.BindInterfacesTo<MainMenuGUIState>().FromComponentInNewPrefab(_menus.MainMenu).AsSingle();

            // лобби вот с такими сложными байдингами, зато на UI это одно и то же меню
            Container.Bind<LobbyGUISettings>().FromComponentSibling();
            Container.Bind<LobbyGUIState>().FromComponentSibling();
            // хотел сделать AsSingle, но тогда Zenject создаёт оба компонента в одном объекте, что для нас не подходит
            Container.BindInterfacesAndSelfTo<ClientLobbyGUIState>().FromNewComponentOnNewPrefab(_menus.LobbyMenu).AsCached();
            Container.BindInterfacesAndSelfTo<ServerLobbyGUIState>().FromNewComponentOnNewPrefab(_menus.LobbyMenu).AsCached();

            Container.BindInterfacesTo<StartClientGUIState>().FromComponentInNewPrefab(_menus.SearchGamesMenu).AsSingle();
            Container.BindInterfacesTo<StartClientDirectIPGUIState>().FromComponentInNewPrefab(_menus.ConnectDirectIpMenu).AsSingle();
            Container.BindInterfacesTo<StartServerGUIState>().FromComponentInNewPrefab(_menus.CreateGameMenu).AsSingle();

            Container.BindInterfacesTo<InGameGUIState>().FromComponentInNewPrefab(_menus.InGameHUD).AsSingle();
            Container.BindInterfacesTo<GameMenuGUIState>().FromComponentInNewPrefab(_menus.GameMenu).AsSingle();

            // менеджер
            Container.BindInterfacesAndSelfTo<GUIManager>()
                .FromComponentInNewPrefab(_menus.GuiManagerPrefab)
                .AsSingle()
                .NonLazy();
        }
    }
}
