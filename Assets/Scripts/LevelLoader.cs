using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class LevelLoader : NetworkBehaviour
{
    public TextAsset[] Levels;
    public GameObject Background;
    public GameObject UnbreakableWallPrefab;

	// Use this for initialization
	void Start ()
	{
        if (!isServer)
            return;

	    var levelIdx = new System.Random().Next(Levels.Length);
	    var level = ParseLevel(Levels[levelIdx]);
	    InstantiateLevel(level);
	}

    private LevelData ParseLevel(TextAsset textAsset)
    {
        var data = new LevelData
        {
            Name = textAsset.name
        };

        var lines = textAsset.text.Split('\n').Select(s => s.Trim('\r')).ToArray();
        var firstLine = lines[0].Split(new[] {' ', '\t'}, StringSplitOptions.RemoveEmptyEntries).ToArray();
        data.Width = int.Parse(firstLine[0]);
        data.Height = int.Parse(firstLine[1]);
        data.Map = new LevelObject[data.Height][];

        for (var lineIdx = 1; lineIdx <= data.Height; lineIdx++)
        {
            var currentLineMap = new LevelObject[data.Width];

            for (var colIdx = 0; colIdx < data.Width; colIdx++)
                currentLineMap[colIdx] = ParseCell(lines, lineIdx, colIdx);

            data.Map[lineIdx-1] = currentLineMap;
        }

        return data;
    }

    private static LevelObject ParseCell(string[] lines, int lineIdx, int colIdx)
    {
        switch (lines[lineIdx][colIdx])
        {
            case ' ':
                return LevelObject.None;
            case '1':
                return LevelObject.RegularWall;
            case 'X':
                return LevelObject.UnbreakableWall;

            default:
                throw new ArgumentException("Неверный формат уровня");
        }
    }

    private void InstantiateLevel(LevelData level)
    {
        // устанавливаем размеры фона
        Background.GetComponent<SpriteRenderer>().size = new Vector2(level.Width*2, level.Height*2);

        for (var i = -level.Width-1; i <= level.Width+1; i++)
        {
            var wall1 = Instantiate(UnbreakableWallPrefab, new Vector3(i, level.Height + 1, 0), Quaternion.identity);
            NetworkServer.Spawn(wall1);
            var wall2 = Instantiate(UnbreakableWallPrefab, new Vector3(i, -level.Height - 1, 0), Quaternion.identity);
            NetworkServer.Spawn(wall2);
        }

        for (var j = -level.Height; j <= level.Height; j++)
        {
            var wall3 = Instantiate(UnbreakableWallPrefab, new Vector3(level.Width + 1, j), Quaternion.identity);
            NetworkServer.Spawn(wall3);
            var wall4 = Instantiate(UnbreakableWallPrefab, new Vector3(-level.Width - 1, j), Quaternion.identity);
            NetworkServer.Spawn(wall4);
        }

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
                    var wall = Instantiate(UnbreakableWallPrefab, new Vector3(-level.Width+j*2+1, -level.Height+i*2+1), Quaternion.identity);
                    NetworkServer.Spawn(wall);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }


    private enum LevelObject
    {
        None,
        RegularWall,
        UnbreakableWall
    }

    private class LevelData
    {
        public string Name { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public LevelObject[][] Map { get; set; }
    }
}
