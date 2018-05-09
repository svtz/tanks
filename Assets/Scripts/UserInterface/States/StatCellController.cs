using UnityEngine;
using UnityEngine.UI;

namespace svtz.Tanks.UserInterface.States
{
    internal sealed class StatCellController : MonoBehaviour
    {
#pragma warning disable 0649
        public Color HighlightedColor;
        public Color NormalColor;
#pragma warning restore 0649

        private Text _text;
        private Outline _outline;

        private bool _initialized = false;
        private void EnsureInitialized()
        {
            if (_initialized)
                return;

            _initialized = true;
            _text = GetComponentInChildren<Text>(true);
            _outline = GetComponentInChildren<Outline>(true);
        }

        public void SetText(string value, bool highlight)
        {
            EnsureInitialized();

            _text.text = value;
            if (highlight)
            {
                _outline.enabled = true;
                _text.color = HighlightedColor;
            }
            else
            {
                _outline.enabled = false;
                _text.color = NormalColor;
            }
        }
    }
}