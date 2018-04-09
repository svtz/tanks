using System;
using System.Linq;
using UnityEngine;

internal sealed class LevelParser
{
    public static LevelData Parse(TextAsset textAsset)
    {
        var data = new LevelData
        {
            Name = textAsset.name
        };

        var lines = textAsset.text.Split('\n').Select(s => s.Trim('\r')).ToArray();
        var firstLine = lines[0].Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries).ToArray();
        data.Width = int.Parse(firstLine[0]);
        data.Height = int.Parse(firstLine[1]);
        data.Map = new LevelObject[data.Height][];

        for (var lineIdx = 1; lineIdx <= data.Height; lineIdx++)
        {
            var currentLineMap = new LevelObject[data.Width];

            for (var colIdx = 0; colIdx < data.Width; colIdx++)
                currentLineMap[colIdx] = ParseCell(lines, lineIdx, colIdx);

            data.Map[lineIdx - 1] = currentLineMap;
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
}