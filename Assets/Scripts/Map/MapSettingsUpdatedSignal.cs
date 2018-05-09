using svtz.Tanks.Infra;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;

namespace svtz.Tanks.Map
{
    internal sealed class MapSettingsUpdatedSignal : Signal<MapSettingsUpdatedSignal, MapSettingsUpdatedSignal.Msg>
    {
        public class Msg : MessageBase
        {
            public float BackgroundWidth { get; set; }
            public float BackgroundHeight { get; set; }
            public Color BackgroundColor { get; set; }
            public Color CrawlerBeltColor { get; set; }

            public override void Serialize(NetworkWriter writer)
            {
                base.Serialize(writer);

                writer.Write(BackgroundWidth);
                writer.Write(BackgroundHeight);
                writer.Write(BackgroundColor);
                writer.Write(CrawlerBeltColor);
            }

            public override void Deserialize(NetworkReader reader)
            {
                base.Deserialize(reader);

                BackgroundWidth = reader.ReadSingle();
                BackgroundHeight = reader.ReadSingle();
                BackgroundColor = reader.ReadColor();
                CrawlerBeltColor = reader.ReadColor();
            }
        }

        public class ServerToClient : ServerToClientSignal<MapSettingsUpdatedSignal, Msg> { }
    }
}