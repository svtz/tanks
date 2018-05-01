using UnityEngine.Networking;
using Zenject;

namespace svtz.Tanks.Network
{
    internal sealed class ConnectedToServerSignal : Signal<ConnectedToServerSignal, NetworkClient> { }
    internal sealed class GameStartedSignal : Signal<GameStartedSignal> { }
}