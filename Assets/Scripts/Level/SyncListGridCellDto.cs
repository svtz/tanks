using UnityEngine;
using UnityEngine.Networking;

internal sealed class SyncListGridCellDto : SyncListStruct<GridCellDto>
{
    protected override void SerializeItem(NetworkWriter writer, GridCellDto item)
    {
        writer.Write(item.Vector2);
        writer.Write((int)item.LevelObject);
    }

    protected override GridCellDto DeserializeItem(NetworkReader reader)
    {
        var v = reader.ReadVector2();
        var o = reader.ReadInt32();

        return new GridCellDto(v, (LevelObject)o);
    }
}