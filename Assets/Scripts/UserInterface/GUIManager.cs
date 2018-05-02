using System.Collections.Generic;
using System.Linq;
using svtz.Tanks.Common;
using svtz.Tanks.Network;
using svtz.Tanks.UserInterface.States;
using UnityEngine;
using Zenject;

namespace svtz.Tanks.UserInterface
{
    internal sealed class GUIManager : MonoBehaviour, IInitializable
    {
        private GUISkin _skin;
        private IGUIState _currentState;
        private Dictionary<GUIState, IGUIState> _guiStates;
        private List<IGUIState> _guiImplementations;
        private GameStartedSignal _gameStartedSignal;
        private DisconnectedFromServerSignal _disconnectedFromServerSignal;

        [Inject]
        public void Construct(
            GameStartedSignal gameStartedSignal,
            DisconnectedFromServerSignal disconnectedFromServerSignal,
            GUISkin guiSkin,
            DiContainer container,
            List<IGUIState> guiStates)
        {
            _skin = guiSkin;
            _gameStartedSignal = gameStartedSignal;
            _guiImplementations = guiStates;
            _disconnectedFromServerSignal = disconnectedFromServerSignal;
        }

        void IInitializable.Initialize()
        {
            _guiStates = _guiImplementations.ToDictionary(s => s.Key);
            _gameStartedSignal.Listen(OnGameStarted);
            _disconnectedFromServerSignal.Listen(OnDisconnected);
            GoToState(GUIState.MainMenu);
        }

        private void OnGameStarted()
        {
            GoToState(GUIState.InGame);
        }

        private void OnDisconnected()
        {
            GoToState(GUIState.MainMenu);
        }

        private void GoToState(GUIState state)
        {
            if (_currentState != null && _currentState.Key == state)
                return;

            _currentState = _guiStates[state];
        }

        private void OnGUI()
        {
            GUI.skin = _skin;
            GoToState(_currentState.OnGUI());
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                OnEscape();
        }

        private void OnEscape()
        {
            GoToState(_currentState.OnEscapePressed());
        }
    }
}