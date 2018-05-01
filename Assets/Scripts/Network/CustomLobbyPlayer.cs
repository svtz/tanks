using UnityEngine;
using UnityEngine.Networking;

namespace svtz.Tanks.Network
{
    public class CustomLobbyPlayer : NetworkLobbyPlayer {
        [SyncVar]
        public string playerName;
   
        // Use this for initialization
        void Start () {
            if (isLocalPlayer)
            {
                CmdSetName(FindObjectOfType<CustomNetworkDiscovery>().playerName);
            }
        }

        [Command]
        public void CmdSetName(string name)
        {
            playerName = name;
        }

        public void DrawGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(playerName, GUILayout.Width(100));
            if (isLocalPlayer)
            {
                if (GUILayout.Button(readyToBegin ? "ГОТОВ" : "Не готов"))
                {
                    readyToBegin = !readyToBegin;
                    if (readyToBegin)
                        SendReadyToBeginMessage();
                    else
                        SendNotReadyToBeginMessage();
                }
            }else
            {
                GUILayout.Label(readyToBegin ? "ГОТОВ" : "Не готов");
            }
            GUILayout.EndHorizontal();
        }
    }
}
