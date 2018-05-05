using System.Collections.Generic;
using svtz.Tanks.Infra;
using UnityEngine.Networking;
using Zenject;

namespace svtz.Tanks.BattleStats
{
    internal sealed class BattleStatsUpdateSignal : Signal<BattleStatsUpdateSignal, BattleStatsUpdateSignal.Msg>
    {
        public sealed class Msg : MessageBase
        {
            public BattleStats BattleStats { get; set; }

            public override void Serialize(NetworkWriter writer)
            {
                base.Serialize(writer);

                var stats = BattleStats.Stats;

                writer.Write(stats.Length);
                foreach (var s in stats)
                {
                    writer.Write(s.Id);
                    writer.Write(s.Frags);
                    writer.Write(s.Name);
                }
            }

            public override void Deserialize(NetworkReader reader)
            {
                base.Deserialize(reader);

                var length = reader.ReadInt32();
                var result = new PlayerStats[length];
                for (var idx = 0; idx < length; ++idx)
                {
                    result[idx] = new PlayerStats
                    {
                        Id = reader.ReadInt32(),
                        Frags = reader.ReadInt32(),
                        Name = reader.ReadString()
                    };
                }

                BattleStats = new BattleStats()
                {
                    Stats = result
                };
            }
        }

        public sealed class ServerToClient : ServerToClientSignal<BattleStatsUpdateSignal, Msg> { }
    }
}
