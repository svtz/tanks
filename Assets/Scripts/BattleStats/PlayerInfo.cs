using svtz.Tanks.Common;
using UnityEngine;
using UnityEngine.Networking;

namespace svtz.Tanks.BattleStats
{
    internal sealed class PlayerInfo : MonoBehaviour
    {
        public IPlayer Player { get; private set; }

        private class PlayerImpl : IPlayer
        {
            public int Id { get; set; }
            public string TeamId { get; set; }
        }

        private void Start()
        {
            Player = new PlayerImpl
            {
                Id = GetComponent<NetworkIdentity>().connectionToClient.connectionId,
                TeamId = GetComponent<TeamId>().Id
            };
        }
    }
}