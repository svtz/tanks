using System.Linq;
using svtz.Tanks.Common;
using UnityEngine.Networking;
using Zenject;

namespace svtz.Tanks.BattleStats
{
    internal sealed class BattleStatsUpdater : IInitializable
    {
        private readonly BattleStatsUpdateSignal.ServerToClient _battleStatsUpdateSignalSender;
        private readonly BattleStatsUpdateSignal _battleStatsUpdateSignal;
        private readonly GameStartedSignal _gameStartedSignal;
        private BattleStats _battleStats;

        private void ServerUpdateBattleStats(BattleStats newStats)
        {
            _battleStats = newStats;
            _battleStatsUpdateSignalSender.FireOnAllClients(new BattleStatsUpdateSignal.Msg {BattleStats = newStats});
        }

        public BattleStatsUpdater(GameStartedSignal gameStartedSignal,
            BattleStatsUpdateSignal.ServerToClient battleStatsUpdateSignalSender,
            BattleStatsUpdateSignal battleStatsUpdateSignal)
        {
            _gameStartedSignal = gameStartedSignal;
            _battleStatsUpdateSignalSender = battleStatsUpdateSignalSender;
            _battleStatsUpdateSignal = battleStatsUpdateSignal;
        }

        private void OnServerGameStarted(GameStartedSignal.Msg msg)
        {
            if (!NetworkServer.active)
            {
                return;
            }

            var players = msg.PlayerNames
                .Select(p => new PlayerStats
                {
                    Deaths = 0,
                    Frags = 0,
                    Id = p.Key,
                    Name = p.Value
                });

            ServerUpdateBattleStats(new BattleStats { Stats = players.ToArray() });
        }

        private void OnClientStatsUpdated(BattleStatsUpdateSignal.Msg msg)
        {
            _battleStats = msg.BattleStats;
        }

        void IInitializable.Initialize()
        {
            _gameStartedSignal.Listen(OnServerGameStarted);
            _battleStatsUpdateSignal.Listen(OnClientStatsUpdated);
        }
    }
}