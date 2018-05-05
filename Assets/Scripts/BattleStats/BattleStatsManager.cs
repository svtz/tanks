using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace svtz.Tanks.BattleStats
{
    internal sealed class BattleStatsManager
    {
        private readonly BattleStatsUpdateSignal.ServerToClient _battleStatsUpdateSignalSender;
        private BattleStats _battleStats;

        private void ServerSendBattleStatsUpdate()
        {
            _battleStatsUpdateSignalSender.FireOnAllClients(new BattleStatsUpdateSignal.Msg {BattleStats = _battleStats});
        }

        public BattleStatsManager(BattleStatsUpdateSignal.ServerToClient battleStatsUpdateSignalSender)
        {
            _battleStatsUpdateSignalSender = battleStatsUpdateSignalSender;
        }

        public void ServerGameStarted(Dictionary<int, string> playerNames)
        {
            var players = playerNames
                .Select(p => new PlayerStats
                {
                    Deaths = 0,
                    Frags = 0,
                    Id = p.Key,
                    Name = p.Value
                })
                .ToDictionary(i => i.Id);

            _battleStats = new BattleStats {Stats = players};
            ServerSendBattleStatsUpdate();
        }

        public void ServerRegisterFrag(GameObject dead, GameObject killer)
        {
            var deadIdentity = dead.gameObject.GetComponent<NetworkIdentity>();
            if (deadIdentity != null && deadIdentity.localPlayerAuthority)
            {
                var id = deadIdentity.connectionToClient.connectionId;
                _battleStats.Stats[id].Deaths++;
            }

            var killerIdentity = killer.gameObject.GetComponent<NetworkIdentity>();
            if (killerIdentity != null && killerIdentity.localPlayerAuthority)
            {
                var id = killerIdentity.connectionToClient.connectionId;
                _battleStats.Stats[id].Frags++;
            }

            ServerSendBattleStatsUpdate();
        }
    }
}