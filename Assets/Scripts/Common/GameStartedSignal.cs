using System.Collections.Generic;
using svtz.Tanks.Infra;
using UnityEngine.Networking;
using Zenject;

namespace svtz.Tanks.Common
{
    internal sealed class GameStartedSignal : Signal<GameStartedSignal, GameStartedSignal.Msg>
    {
        public class Msg : MessageBase
        {
            public Dictionary<int, string> PlayerNames { get; set; }

            public override void Deserialize(NetworkReader reader)
            {
                base.Deserialize(reader);

                var size = reader.ReadInt32();
                PlayerNames = new Dictionary<int, string>(size);

                for (var idx = 0; idx < size; ++idx)
                {
                    PlayerNames.Add(reader.ReadInt32(), reader.ReadString());
                }
            }

            public override void Serialize(NetworkWriter writer)
            {
                base.Serialize(writer);

                writer.Write(PlayerNames.Count);
                foreach (var kv in PlayerNames)
                {
                    writer.Write(kv.Key);
                    writer.Write(kv.Value);
                }
            }
        }

        public class ServerToClient : ServerToClientSignal<GameStartedSignal, Msg>
        {
        }
    }
}