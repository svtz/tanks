using System;
using svtz.Tanks.Assets.Scripts.Map;
using UnityEngine;

namespace svtz.Tanks.Assets.Scripts
{
    internal sealed class MapLoader : MonoBehaviour
    {
#pragma warning disable 0649
        public TextAsset[] AvailableMaps;
#pragma warning restore 0649

        // Use this for initialization
        private void Start()
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

            var controller = FindObjectOfType<MapObjectsController>();

            // строим периметр
            for (var i = -map.Width / 2 - 1; i <= map.Width / 2; i++)
            {
                controller.Add(i + 0.5f, map.Height / 2.0f + 0.5f, MapObjectKind.UnbreakableWall);
                controller.Add(i + 0.5f, -map.Height / 2.0f - 0.5f, MapObjectKind.UnbreakableWall);
            }

            for (var j = -map.Height / 2; j <= map.Height / 2 - 1; j++)
            {
                controller.Add(map.Width / 2.0f + 0.5f, j + 0.5f, MapObjectKind.UnbreakableWall);
                controller.Add(-map.Width / 2.0f - 0.5f, j + 0.5f, MapObjectKind.UnbreakableWall);
            }

            // спавним остальное
            for (var i = 0; i < map.Height; i++)
            for (var j = 0; j < map.Width; j++)
            {
                var x = -map.Width / 2 + j + 0.5f;
                var y = -map.Height / 2 + i + 0.5f;

                switch (map.Map[i][j])
                {
                    case MapObjectKind.None:
                        break;
                    case MapObjectKind.RegularWall:
                        controller.Add(x, y, MapObjectKind.RegularWall);
                        break;
                    case MapObjectKind.UnbreakableWall:
                        controller.Add(x, y, MapObjectKind.UnbreakableWall);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}