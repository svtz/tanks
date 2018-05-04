using svtz.Tanks.Network;
using UnityEngine.Networking;
using Zenject;

namespace svtz.Tanks.Tank
{
    internal sealed class RespawningSignal : Signal<RespawningSignal, float>
    {
        private ConnectedToServerSignal _connectedToServer;
        private NetworkClient _currentClient;
        private const int RespawningMessageCode = 2144;

        [Inject]
        private void Construct(ConnectedToServerSignal connectedToServer)
        {
            _connectedToServer = connectedToServer;
            _connectedToServer.Listen(InitializeRespawnedMessages);
        }

        private void InitializeRespawnedMessages(NetworkClient newClient)
        {
            if (newClient == _currentClient)
                return;

            if (_currentClient != null)
            {
                _currentClient.UnregisterHandler(RespawningMessageCode);
            }

            _currentClient = newClient;
            _currentClient.RegisterHandler(RespawningMessageCode, OnRespawning);
        }

        public void FireOnClient(NetworkConnection client, float time)
        {
            client.Send(RespawningMessageCode, new RespawningMessage {Time = time});
        }

        private void OnRespawning(NetworkMessage netmsg)
        {
            var msg = netmsg.ReadMessage<RespawningMessage>();
            Fire(msg.Time);
        }

        private class RespawningMessage : MessageBase
        {
            public float Time { get; set; }

            public override void Serialize(NetworkWriter writer)
            {
                base.Serialize(writer);
                writer.Write(Time);
            }

            public override void Deserialize(NetworkReader reader)
            {
                base.Deserialize(reader);

                Time = reader.ReadSingle();
            }
        }
    }
}