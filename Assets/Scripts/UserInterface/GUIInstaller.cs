using System;
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
            public GameObject MainMenu;
            public GameObject SearchGamesMenu;
            public GameObject CreateGameMenu;
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

            //Container.BindInterfacesTo<ClientLobbyGUIState>().AsSingle();
            //Container.BindInterfacesTo<ServerLobbyGUIState>().AsSingle();

            Container.BindInterfacesTo<StartClientGUIState>().FromComponentInNewPrefab(_menus.SearchGamesMenu).AsSingle();
            Container.BindInterfacesTo<StartServerGUIState>().FromComponentInNewPrefab(_menus.CreateGameMenu).AsSingle();

            //Container.BindInterfacesTo<InGameGUIState>().AsSingle();
            //Container.BindInterfacesTo<GameMenuGUIState>().AsSingle();

            // менеджер
            Container.BindInterfacesAndSelfTo<GUIManager>()
                .FromNewComponentOnNewGameObject()
                .AsSingle()
                .NonLazy();
        }
    }
}
