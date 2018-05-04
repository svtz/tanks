using svtz.Tanks.Tank;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace svtz.Tanks.UserInterface.States
{
    internal sealed class RespawnCountdownDisplay : MonoBehaviour
    {
#pragma warning disable 0649
        public RectTransform CountdownText;
#pragma warning restore 0649

        private const string CountdownFormat = "Новый подгонят через {0:F1} секунды";

        private RespawningSignal _respawningSignal;
        private Text _countdownText;
        private bool _active = false;
        private float _timeRemaining;


        [Inject]
        private void Construct(RespawningSignal respawningSignal)
        {
            _respawningSignal = respawningSignal;
            _respawningSignal.Listen(Run);
        }

        private void Start()
        {
            _countdownText = CountdownText.GetComponent<Text>();
        }

        private void Run(float time)
        {
            Debug.Assert(!_active);

            _timeRemaining = time;
            _active = true;
            gameObject.SetActive(true);
        }

        private void Update()
        {
            Debug.Assert(_active);

            _timeRemaining -= Time.deltaTime;
            if (_timeRemaining > 0)
            {
                _countdownText.text = string.Format(CountdownFormat, _timeRemaining);
            }
            else
            {
                gameObject.SetActive(false);
                _active = false;
            }
        }
    }
}