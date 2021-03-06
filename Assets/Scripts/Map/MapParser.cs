﻿using System;
using System.Linq;
using UnityEngine;

namespace svtz.Tanks.Map
{
    internal sealed class MapParser
    {
        public MapInfo Parse(TextAsset textAsset)
        {
            var data = new MapInfo
            {
                Name = textAsset.name
            };

            var lines = textAsset.text.Split('\n').Select(s => s.Trim('\r')).ToArray();
            var firstLine = lines[0].Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries).ToArray();
            data.Width = int.Parse(firstLine[0]);
            data.Height = int.Parse(firstLine[1]);
            Color color;
            if (firstLine.Length > 2 && ColorUtility.TryParseHtmlString(firstLine[2], out color))
            {
                data.BackgroundColor = color;
            }
            if (firstLine.Length > 3 && ColorUtility.TryParseHtmlString(firstLine[3], out color))
            {
                data.CrawlerBeltColor = color;
            }

            data.Map = new MapObjectKind[data.Height][];

            for (var lineIdx = 1; lineIdx <= data.Height; lineIdx++)
            {
                var currentLineMap = new MapObjectKind[data.Width];

                for (var colIdx = 0; colIdx < data.Width; colIdx++)
                    currentLineMap[colIdx] = ParseCell(lines, lineIdx, colIdx);

                data.Map[lineIdx - 1] = currentLineMap;
            }

            // переворачиваем карту
            data.Map = data.Map.Reverse().ToArray();

            return data;
        }

        private static MapObjectKind ParseCell(string[] lines, int lineIdx, int colIdx)
        {
            switch (lines[lineIdx][colIdx])
            {
                case ' ':
                    return MapObjectKind.None;
                case '*':
                    return MapObjectKind.RegularWall;
                case 'X':
                    return MapObjectKind.ArmoredWall;
                case 'T':
                    return MapObjectKind.Tree;
                case 'S':
                    return MapObjectKind.RandomPlayerSpawn;
                case '?':
                    return MapObjectKind.RandomBonusSpawner;
               
                default:
                    throw new ArgumentException(string.Format("Неверный формат уровня ({0},{1})", lineIdx, colIdx));
            }
        }
    }
}