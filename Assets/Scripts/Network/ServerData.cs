using System;

namespace svtz.Tanks.Network
{
    public class ServerData
    {
        public string Key { get; private set; }

        public string NetworkAddress { get; private set; }
        public int Port { get; private set; }
        public string ServerName { get; private set; }
        public int CurrentPlayersCount { get; private set; }
        public bool Online { get; private set; }

        public static ServerData Create(string name, string address, int port)
        {
            return new ServerData
            {
                ServerName = name,
                NetworkAddress = address,
                Port = port,
                Key = string.Format("{0}:{1}", address, port)
            };
        }

        public static ServerData Parse(string data)
        {
            var fields = data.Split(':');
            var sd = new ServerData
            {
                NetworkAddress = fields[0],
                Port = int.Parse(fields[1]),
                ServerName = fields[2],
                CurrentPlayersCount = int.Parse(fields[3]),
                Online = int.Parse(fields[4]) == 1
            };
            sd.Key = string.Format("{0}:{1}", sd.NetworkAddress, sd.Port);

            return sd;
        }

        public bool SameServerAs(ServerData other)
        {
            return other != null 
                && string.Equals(Key, other.Key, StringComparison.OrdinalIgnoreCase);
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
}
