using System.Collections.Generic;
using UnityEngine.Networking;
using Zenject;

namespace svtz.Tanks.Common
{
    internal sealed class TeamManager : IInitializable
    {
        private readonly Dictionary<NetworkConnection, string> _teamByConnection
            = new Dictionary<NetworkConnection, string>();

        private string _allyTeam;

        private const int TeamIdMessageCode = 2143;

        private void OnTeamIdReceived(NetworkMessage netmsg)
        {
            var msg = netmsg.ReadMessage<TeamIdMessage>();
            _allyTeam = msg.TeamId;
        }

        public void Initialize()
        {
            NetworkManager.singleton.client.RegisterHandler(TeamIdMessageCode, OnTeamIdReceived);
        }

        private class TeamIdMessage : MessageBase
        {
            public string TeamId { get; set; }

            public override void Serialize(NetworkWriter writer)
            {
                base.Serialize(writer);
                writer.Write(TeamId);
            }

            public override void Deserialize(NetworkReader reader)
            {
                base.Deserialize(reader);

                TeamId = reader.ReadString();
            }
        }

        public void RegisterPlayer(NetworkConnection connection)
        {
            var id = (_teamByConnection.Count + 1).ToString();
            _teamByConnection.Add(connection, id);

            connection.Send(TeamIdMessageCode, new TeamIdMessage {TeamId = id});
        }

        public string GetTeamForConnection(NetworkConnection connection)
        {
            return _teamByConnection[connection];
        }

        public bool IsAlly(string teamId)
        {
            return teamId == _allyTeam;
        }

    }
}