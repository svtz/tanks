using System;
using UnityEngine;
using UnityEngine.Networking;
using Random = UnityEngine.Random;

namespace svtz.Tanks.Assets.Scripts.Map
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
        private readonly MapObjectsController _mapObjectsController;
        private readonly SpawnController _spawnController;

        public MapCreator(Settings settings, 
            MapParser mapParser, 
            MapObjectsController mapObjectsController,
            SpawnController spawnController)
        {
            _settings = settings;
            _mapParser = mapParser;
            _mapObjectsController = mapObjectsController;
            _spawnController = spawnController;
        }

        public void Create()
        {
            var mapIdx = Random.Range(0, _settings.AvailableMaps.Length);
            var mapInfo = _mapParser.Parse(_settings.AvailableMaps[mapIdx]);
            InstantiateMapObjects(mapInfo);
        }

        private void InstantiateMapObjects(MapInfo map)
        {
            // устанавливаем размеры фона
            _mapObjectsController.SetSize(map.Width, map.Height);

            // строим периметр
            for (var i = -map.Width / 2 - 1; i <= map.Width / 2; i++)
            {
                _mapObjectsController.Add(i + 0.5f, map.Height / 2.0f + 0.5f, MapObjectKind.UnbreakableWall);
                _mapObjectsController.Add(i + 0.5f, -map.Height / 2.0f - 0.5f, MapObjectKind.UnbreakableWall);
            }

            for (var j = -map.Height / 2; j <= map.Height / 2 - 1; j++)
            {
                _mapObjectsController.Add(map.Width / 2.0f + 0.5f, j + 0.5f, MapObjectKind.UnbreakableWall);
                _mapObjectsController.Add(-map.Width / 2.0f - 0.5f, j + 0.5f, MapObjectKind.UnbreakableWall);
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
                    case MapObjectKind.RegularWall:
                        _mapObjectsController.Add(-map.Width / 2 + j + 0.5f, -map.Height / 2 + i + 0.5f, cellKind);
                        break;

                    case MapObjectKind.RandomPlayerSpawn:
                        _spawnController.AddSpawnPoint(-map.Width / 2 + j + 1.0f, -map.Height / 2 + i);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}