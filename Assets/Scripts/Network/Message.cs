using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace svtz.Tanks.Network
{
    public class Message : MonoBehaviour
    {
        public string text;
        public int width = 300;
        public int height = 200;
        private Rect wndRect;
        private void Start()
        {
            wndRect = new Rect(
                (Screen.width - width) / 2,
                (Screen.height - height) / 2,
                width,
                height);
        }

        private void OnGUI()
        {
            GUI.Box(wndRect, "");
            GUILayout.BeginArea(wndRect);
            GUILayout.BeginVertical();
            GUILayout.Label(text);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("OK", GUILayout.Width(100)))
                Destroy(gameObject);
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }
    }
}
