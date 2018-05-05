using UnityEngine;
using Zenject;

namespace svtz.Tanks.UserInterface.States
{
    internal sealed class LobbyGUISettings : MonoBehaviour
    {
#pragma warning disable 0649
        public RectTransform PlayerItemPrefab;
        public RectTransform ScrollContent;
        public float RefreshIntervalSeconds;
#pragma warning restore 0649

        private LobbyGUIState _state;

        [Inject]
        public void Construct(LobbyGUIState state)
        {
            _state = state;
        }

        public void OnEscape()
        {
            _state.DoOnEscape();
        }
    }
}