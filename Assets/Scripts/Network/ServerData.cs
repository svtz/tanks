using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerData : ScriptableObject {
    public string NetworkAddress;
    public int Port;
    public string ServerName;
    public int CurrentPlayersCount;
    public bool Online;
    public void Load(string data)
    {
        string[] fields = data.Split(':');
        NetworkAddress = fields[0];
        Port = int.Parse(fields[1]);
        ServerName = fields[2];
        CurrentPlayersCount = int.Parse(fields[3]);
        Online = int.Parse(fields[4]) == 1;
    }
    public override string ToString()
    {
        return
            NetworkAddress + ':' +
            Port + ':' +
            ServerName + ':' +
            CurrentPlayersCount + ':' +
            (Online? '1':'0');
    }

    
}
