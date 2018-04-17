using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CustomLobbyPlayer : NetworkLobbyPlayer {
    [SyncVar]
    public string playerName;
   
    // Use this for initialization
    void Start () {
	    if (isLocalPlayer)
        {
            CmdSetName(NetworkManager.singleton.GetComponent<CustomNetworkDiscovery>().playerName);
        }
	}

    [Command]
    public void CmdSetName(string name)
    {
        playerName = name;
    }

    // Update is called once per frame
    void Update () {
		
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
