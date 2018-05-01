using svtz.Tanks.UserInterface.States;
using UnityEngine;
using Zenject;

namespace svtz.Tanks.UserInterface
{
    internal sealed class GUIInstaller : Installer<GUISkin, GUIInstaller>
    {
        private readonly GUISkin _skin;

        public GUIInstaller(GUISkin skin)
        {
            _skin = skin;
        }

        public override void InstallBindings()
        {
            Container.BindInstance(_skin);

            // состояния
            Container.BindInterfacesTo<MainMenuGUIState>().AsSingle();

            Container.BindInterfacesTo<ClientLobbyGUIState>().AsSingle();
            Container.BindInterfacesTo<ServerLobbyGUIState>().AsSingle();

            Container.BindInterfacesTo<StartClientGUIState>().AsSingle();
            Container.BindInterfacesTo<StartServerGUIState>().AsSingle();

            Container.BindInterfacesTo<InGameGUIState>().AsSingle();
            Container.BindInterfacesTo<GameMenuGUIState>().AsSingle();

            // менеджер
            Container.BindInterfacesAndSelfTo<GUIManager>()
                .FromNewComponentOnNewGameObject()
                .AsSingle()
                .NonLazy();
        }
    }
}
