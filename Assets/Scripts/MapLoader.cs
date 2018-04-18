using System;
using svtz.Tanks.Assets.Scripts.Map;
using UnityEngine;
using UnityEngine.Networking;

namespace svtz.Tanks.Assets.Scripts
{
    internal sealed class MapLoader : NetworkBehaviour
    {
#pragma warning disable 0649
        public TextAsset[] AvailableMaps;
#pragma warning restore 0649

        // Use this for initialization
        public override void OnStartServer()
        {
            var mapIdx = new System.Random().Next(AvailableMaps.Length);
            var mapInfo = MapParser.Parse(AvailableMaps[mapIdx]);
            InstantiateMapObjects(mapInfo);
        }

        private void InstantiateMapObjects(MapInfo map)
        {
            var backgroundController = FindObjectOfType<BackgroundSizeController>();
            // устанавливаем размеры фона
            backgroundController.SetSize(map.Width, map.Height);

            var mapObjectsController = FindObjectOfType<MapObjectsController>();

            // строим периметр
            for (var i = -map.Width / 2 - 1; i <= map.Width / 2; i++)
            {
                mapObjectsController.Add(i + 0.5f, map.Height / 2.0f + 0.5f, MapObjectKind.UnbreakableWall);
                mapObjectsController.Add(i + 0.5f, -map.Height / 2.0f - 0.5f, MapObjectKind.UnbreakableWall);
            }

            for (var j = -map.Height / 2; j <= map.Height / 2 - 1; j++)
            {
                mapObjectsController.Add(map.Width / 2.0f + 0.5f, j + 0.5f, MapObjectKind.UnbreakableWall);
                mapObjectsController.Add(-map.Width / 2.0f - 0.5f, j + 0.5f, MapObjectKind.UnbreakableWall);
            }

            // спавним остальное
            var spawnController = FindObjectOfType<SpawnController>();
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
                        mapObjectsController.Add(-map.Width / 2 + j + 0.5f, -map.Height / 2 + i + 0.5f, cellKind);
                        break;

                    case MapObjectKind.RandomPlayerSpawn:
                        spawnController.Add(-map.Width / 2 + j + 1.0f, -map.Height / 2 + i);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}