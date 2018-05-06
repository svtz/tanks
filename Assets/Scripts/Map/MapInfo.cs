using UnityEngine;

namespace svtz.Tanks.Map
{
    internal sealed class MapInfo
    {
        public string Name { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Color Color { get; set; }
        public MapObjectKind[][] Map { get; set; }
    }
}