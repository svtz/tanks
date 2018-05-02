using System;
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

        protected void CenterScreen(Action layout)
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
    }
}