using System;
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
        private IGUIState _currentState;
        private Dictionary<GUIState, IGUIState> _guiStates;
        private List<IGUIState> _guiImplementations;
        private GameStartedSignal _gameStartedSignal;
        private DisconnectedFromServerSignal _disconnectedFromServerSignal;

        [Inject]
        public void Construct(
            GameStartedSignal gameStartedSignal,
            DisconnectedFromServerSignal disconnectedFromServerSignal,
            List<IGUIState> guiStates)
        {
            _gameStartedSignal = gameStartedSignal;
            _guiImplementations = guiStates;
            _disconnectedFromServerSignal = disconnectedFromServerSignal;
        }

        void IInitializable.Initialize()
        {
            _guiStates = _guiImplementations.ToDictionary(s => s.Key);
            _gameStartedSignal.Listen(() => GoToState(GUIState.InGame));
            _disconnectedFromServerSignal.Listen(() => GoToState(GUIState.MainMenu));
            GoToState(GUIState.MainMenu);
        }

        private bool _lockTransitions = false;

        public void GoToState(GUIState state)
        {
            if (_lockTransitions)
                throw new InvalidOperationException("Попытка изменения UI-состояния в момент открытия-закрытия другого состояния.");

            _lockTransitions = true;
            try
            {
                if (_currentState != null)
                {
                    if (_currentState.Key == state)
                        return;

                    _currentState.OnExitState();
                }

                _currentState = _guiStates[state];
                _currentState.OnEnterState();
            }
            finally
            {
                _lockTransitions = false;
            }
        }
    }
}