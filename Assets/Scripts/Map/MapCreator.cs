﻿using System;
using svtz.Tanks.Bonus;
using svtz.Tanks.Common;
using svtz.Tanks.Tank;
using UnityEngine;
using Random = UnityEngine.Random;

namespace svtz.Tanks.Map
{
    internal sealed class MapCreator
    {
        [Serializable]
        public class Settings
        {
#pragma warning disable 0649
            public TextAsset[] AvailableMaps;
#pragma warning restore 0649
        }

        private readonly Settings _settings;
        private readonly MapParser _mapParser;
        private readonly MapObjectsManager _mapObjectsManager;
        private readonly TankSpawner _tankSpawner;
        private readonly BonusSpawner _bonusSpawner;
        private readonly MapSettingsManager _mapSettingsManager;

        public MapCreator(Settings settings, 
            MapParser mapParser, 
            MapObjectsManager mapObjectsManager,
            TankSpawner tankSpawner,
            BonusSpawner bonusSpawner,
            MapSettingsManager mapSettingsManager)
        {
            _settings = settings;
            _mapParser = mapParser;
            _mapObjectsManager = mapObjectsManager;
            _tankSpawner = tankSpawner;
            _bonusSpawner = bonusSpawner;
            _mapSettingsManager = mapSettingsManager;
        }

        public void Create()
        {
            var mapIdx = Random.Range(0, _settings.AvailableMaps.Length);
            var mapInfo = _mapParser.Parse(_settings.AvailableMaps[mapIdx]);
            InstantiateMapObjects(mapInfo);
        }

        private void InstantiateMapObjects(MapInfo map)
        {
            // устанавливаем параметры карты
            _mapSettingsManager.ServerUpdate(map);

            // строим периметр
            for (var i = -map.Width / 2 - 1; i <= map.Width / 2; i++)
            {
                _mapObjectsManager.Add(i + 0.5f, map.Height / 2.0f + 0.5f, MapObjectKind.UnbreakableWall);
                _mapObjectsManager.Add(i + 0.5f, -map.Height / 2.0f - 0.5f, MapObjectKind.UnbreakableWall);
            }

            for (var j = -map.Height / 2; j <= map.Height / 2 - 1; j++)
            {
                _mapObjectsManager.Add(map.Width / 2.0f + 0.5f, j + 0.5f, MapObjectKind.UnbreakableWall);
                _mapObjectsManager.Add(-map.Width / 2.0f - 0.5f, j + 0.5f, MapObjectKind.UnbreakableWall);
            }

            // спавним остальное
            for (var i = 0; i < map.Height; i++)
            for (var j = 0; j < map.Width; j++)
            {
                var cellKind = map.Map[i][j];
                switch (cellKind)
                {
                    case MapObjectKind.None:
                        break;

                    case MapObjectKind.UnbreakableWall:
                    case MapObjectKind.ArmoredWall:
                    case MapObjectKind.RegularWall:
                    case MapObjectKind.Tree:
                        _mapObjectsManager.Add(-map.Width / 2 + j + 0.5f, -map.Height / 2 + i + 0.5f, cellKind);
                        break;

                    case MapObjectKind.RandomPlayerSpawn:
                        _tankSpawner.AddSpawnPoint(-map.Width / 2 + j + 1.0f, -map.Height / 2 + i + 1.0f);
                        break;

                    case MapObjectKind.RandomBonusSpawner:
                        _bonusSpawner.ServerAddSpawnPoint(-map.Width / 2 + j + 1.0f, -map.Height / 2 + i + 1.0f);
                        break;
                        
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}