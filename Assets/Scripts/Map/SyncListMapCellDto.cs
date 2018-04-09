using UnityEngine.Networking;

namespace svtz.Tanks.Assets.Scripts.Map
{
    internal sealed class SyncListMapCellDto : SyncListStruct<MapCellDto>
    {
        protected override void SerializeItem(NetworkWriter writer, MapCellDto item)
        {
            writer.Write(item.Vector2);
            writer.Write((int)item.MapObjectKind);
        }

        protected override MapCellDto DeserializeItem(NetworkReader reader)
        {
            var v = reader.ReadVector2();
            var o = reader.ReadInt32();

            return new MapCellDto(v, (MapObjectKind)o);
        }
    }
}