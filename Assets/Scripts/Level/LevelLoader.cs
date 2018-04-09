using System;
using UnityEngine;
using UnityEngine.Networking;

public class LevelLoader : NetworkBehaviour
{
    public TextAsset[] Levels;
    public GameObject Background;
    public GameObject UnbreakableWallPrefab;

    [SyncVar]
    private int _width;
    [SyncVar]
    private int _height;

    private readonly SyncListGridCellDto _map = new SyncListGridCellDto();
    private void UnbreakableWallsChanged(SyncListGridCellDto.Operation op, int itemIndex)
    {
        switch (op)
        {
            case SyncListGridCellDto.Operation.OP_ADD:
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
	    _map.Callback += UnbreakableWallsChanged;
        if (!isServer)
        {
            for (var i = 0; i < _map.Count; ++i)
            {
                UnbreakableWallsChanged(SyncList<GridCellDto>.Operation.OP_ADD, i);
            }

            InitBackgroundSize();
        }

	    if (!isServer)
            return;

	    var levelIdx = new System.Random().Next(Levels.Length);
	    var level = LevelParser.Parse(Levels[levelIdx]);
	    InstantiateLevel(level);
	}

    private void InstantiateLevel(LevelData level)
    {
        // устанавливаем размеры фона
        _width = level.Width;
        _height = level.Height;
        InitBackgroundSize();

        // строим периметр
        for (var i = -level.Width-1; i <= level.Width+1; i++)
        {
            _map.Add(new GridCellDto(new Vector2(i, level.Height + 1), LevelObject.UnbreakableWall));
            _map.Add(new GridCellDto(new Vector2(i, -level.Height - 1), LevelObject.UnbreakableWall));
        }

        for (var j = -level.Height; j <= level.Height; j++)
        {
            _map.Add(new GridCellDto(new Vector2(level.Width + 1, j), LevelObject.UnbreakableWall));
            _map.Add(new GridCellDto(new Vector2(-level.Width - 1, j), LevelObject.UnbreakableWall));
        }

        // спавним остальное
        for (var i = 0; i < level.Height; i++)
        for (var j = 0; j < level.Width; j++)
        {
            switch (level.Map[i][j])
            {
                case LevelObject.None:
                    break;
                case LevelObject.RegularWall:
                    break;
                case LevelObject.UnbreakableWall:
                    _map.Add(new GridCellDto(new Vector2(-level.Width + j * 2 + 1, -level.Height + i * 2 + 1), LevelObject.UnbreakableWall));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

}