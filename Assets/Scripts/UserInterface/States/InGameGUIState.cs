//using UnityEngine;

//namespace svtz.Tanks.UserInterface.States
//{
//    internal sealed class InGameGUIState : AbstractGUIState
//    {
//        public InGameGUIState(GUISkin guiSkin) : base(guiSkin)
//        {
//        }

//        public override GUIState Key
//        {
//            get { return GUIState.InGame; }
//        }

//        public override GUIState OnGUI()
//        {
//            return Key;
//        }

//        public override GUIState OnEscapePressed()
//        {
//            return GUIState.GameMenu;
//        }
//    }
//}