using System;
using UnityEngine;
using UnityEngine.Networking;

public class MapLoader : NetworkBehaviour
{
    public TextAsset[] AvailableMaps;
    public GameObject Background;
    public GameObject UnbreakableWallPrefab;

    [SyncVar]
    private int _width;
    [SyncVar]
    private int _height;

    private readonly SyncListMapCellDto _map = new SyncListMapCellDto();
    private void MapChanged(SyncListMapCellDto.Operation op, int itemIndex)
    {
        switch (op)
        {
            case SyncListMapCellDto.Operation.OP_ADD:
                Instantiate(UnbreakableWallPrefab, _map[itemIndex].Vector2, Quaternion.identity);
                break;
            default:
                throw new ArgumentOutOfRangeException("op", op, null);
        }
    }

    private void InitBackgroundSize()
    {
        Background.GetComponent<SpriteRenderer>().size = new Vector2(_width * 2, _height * 2);
    }

    // Use this for initialization
    private void Start()
	{
	    _map.Callback += MapChanged;
        if (!isServer)
        {
            for (var i = 0; i < _map.Count; ++i)
            {
                MapChanged(SyncList<MapCellDto>.Operation.OP_ADD, i);
            }

            InitBackgroundSize();
        }

	    if (!isServer)
            return;

	    var mapIdx = new System.Random().Next(AvailableMaps.Length);
	    var mapInfo = LevelParser.Parse(AvailableMaps[mapIdx]);
	    InstantiateMapObjects(mapInfo);
	}

    private void InstantiateMapObjects(MapInfo map)
    {
        // устанавливаем размеры фона
        _width = map.Width;
        _height = map.Height;
        InitBackgroundSize();

        // строим периметр
        for (var i = -map.Width-1; i <= map.Width+1; i++)
        {
            _map.Add(new MapCellDto(new Vector2(i, map.Height + 1), MapObjectKind.UnbreakableWall));
            _map.Add(new MapCellDto(new Vector2(i, -map.Height - 1), MapObjectKind.UnbreakableWall));
        }

        for (var j = -map.Height; j <= map.Height; j++)
        {
            _map.Add(new MapCellDto(new Vector2(map.Width + 1, j), MapObjectKind.UnbreakableWall));
            _map.Add(new MapCellDto(new Vector2(-map.Width - 1, j), MapObjectKind.UnbreakableWall));
        }

        // спавним остальное
        for (var i = 0; i < map.Height; i++)
        for (var j = 0; j < map.Width; j++)
        {
            switch (map.Map[i][j])
            {
                case MapObjectKind.None:
                    break;
                case MapObjectKind.RegularWall:
                    break;
                case MapObjectKind.UnbreakableWall:
                    _map.Add(new MapCellDto(new Vector2(-map.Width + j * 2 + 1, -map.Height + i * 2 + 1), MapObjectKind.UnbreakableWall));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

}