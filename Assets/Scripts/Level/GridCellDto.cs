using UnityEngine;

internal struct GridCellDto
{
    public Vector2 Vector2 { get; private set; }
    public LevelObject LevelObject { get; private set; }

    public GridCellDto(Vector2 vector2, LevelObject obj) : this()
    {
        Vector2 = vector2;
        LevelObject = obj;
    }
}