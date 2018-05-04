using System.Collections.Generic;
using svtz.Tanks.Infra;
using svtz.Tanks.Network;
using UnityEngine.Networking;
using Zenject;

namespace svtz.Tanks.Common
{
    internal sealed class TeamManager
    {
        private readonly Dictionary<NetworkConnection, string> _teamByConnection
            = new Dictionary<NetworkConnection, string>();

        private string _allyTeam;

        private ConnectedToServerSignal _connectedToServer;
        private NetworkClient _currentClient;

        [Inject]
        private void Construct(ConnectedToServerSignal connectedToServer)
        {
            _connectedToServer = connectedToServer;
            _connectedToServer.Listen(InitializeTeamIdMessages);
        }

        private void InitializeTeamIdMessages(NetworkClient newClient)
        {
            if (newClient == _currentClient)
                return;

            if (_currentClient != null)
            {
                _currentClient.UnregisterHandler(MessageCodes.TeamId);
                _allyTeam = null;
            }

            _currentClient = newClient;
            _currentClient.RegisterHandler(MessageCodes.TeamId, OnTeamIdReceived);
        }

        private void OnTeamIdReceived(NetworkMessage netmsg)
        {
            var msg = netmsg.ReadMessage<TeamIdMessage>();
            _allyTeam = msg.TeamId;
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

            connection.Send(MessageCodes.TeamId, new TeamIdMessage {TeamId = id});
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