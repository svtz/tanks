using svtz.Tanks.Infra;
using UnityEngine.Networking;
using Zenject;

namespace svtz.Tanks.Tank
{
    internal sealed class RespawningSignal : Signal<RespawningSignal, RespawningSignal.Msg>
    {
        public class Msg : MessageBase
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

        internal sealed class ServerToClient : ServerToClientSignal<RespawningSignal, Msg>
        {
        }
    }
}