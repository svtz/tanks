using UnityEngine;

namespace svtz.Tanks.UserInterface.States
{
    internal abstract class AbstractGUIState : IGUIState
{
        private readonly GUISkin _guiSkin;

        public abstract GUIState Key { get; }
        public abstract GUIState OnGUI();
        public abstract GUIState OnEscapePressed();

        protected AbstractGUIState(GUISkin guiSkin)
        {
            _guiSkin = guiSkin;
        }

        protected GUIStyle GetStyle(string name)
        {
            return _guiSkin.GetStyle(name);
        }

        protected Rect GamePanel()
        {
            var style = GetStyle("MenuArea");
            return new Rect(
                (Screen.width - style.fixedWidth) / 2,
                (Screen.height - style.fixedHeight) / 2,
                style.fixedWidth,
                style.fixedHeight);
        }
    }
}