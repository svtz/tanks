using System;
using UnityEngine;

namespace svtz.Tanks.UserInterface.States
{
    internal abstract class AbstractGUIState : IGUIState
    {
        protected GUISkin GuiSkin { get; private set; }

        public abstract GUIState Key { get; }
        public abstract GUIState OnGUI();
        public abstract GUIState OnEscapePressed();

        protected AbstractGUIState(GUISkin guiSkin)
        {
            GuiSkin = guiSkin;
        }

        protected GUIStyle GetStyle(string name)
        {
            return GuiSkin.GetStyle(name);
        }

        protected void Center(Action layout)
        {
            GUILayout.BeginArea(Screen.safeArea);
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.BeginVertical(GetStyle("MenuArea"));

            layout();

            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.EndArea();
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