using System.Linq;
using svtz.Tanks.Network;
using UnityEngine.Networking;
using Zenject;

namespace svtz.Tanks.Infra
{
    internal abstract class ServerToClientSignal<TLocalSignal, TMessage>
        where TLocalSignal : Signal<TLocalSignal, TMessage>
        where TMessage : MessageBase, new()
    {
        private TLocalSignal _localSignal;
        private ConnectedToServerSignal _connectedToServer;
        private NetworkClient _currentClient;
        private short _messageCode;

        [Inject]
        private void Construct(
            TLocalSignal localSignal,
            ConnectedToServerSignal connectedToServer,
            short code)
        {
            _localSignal = localSignal;
            _messageCode = code;
            _connectedToServer = connectedToServer;
            _connectedToServer.Listen(InitializeMessages);
        }

        private void InitializeMessages(NetworkClient newClient)
        {
            if (newClient == _currentClient)
                return;

            if (_currentClient != null)
            {
                _currentClient.UnregisterHandler(_messageCode);
            }

            _currentClient = newClient;
            _currentClient.RegisterHandler(_messageCode, OnMessage);
        }

        private void OnMessage(NetworkMessage netmsg)
        {
            var msg = netmsg.ReadMessage<TMessage>();
            _localSignal.Fire(msg);
        }

        public virtual void FireOnClient(NetworkConnection client, TMessage message)
        {
            client.Send(_messageCode, message);
        }

        public void FireOnAllClients(TMessage message)
        {
            foreach (var client in NetworkServer.connections)
            {
                FireOnClient(client, message);
            }
        }
    }
}