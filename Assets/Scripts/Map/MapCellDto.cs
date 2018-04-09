using UnityEngine;

internal struct MapCellDto
{
    public Vector2 Vector2 { get; private set; }
    public MapObjectKind MapObjectKind { get; private set; }

    public MapCellDto(Vector2 vector2, MapObjectKind obj) : this()
    {
        Vector2 = vector2;
        MapObjectKind = obj;
    }
}