namespace svtz.Tanks.Assets.Scripts.Map
{
    internal sealed class MapInfo
    {
        public string Name { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public MapObjectKind[][] Map { get; set; }
    }
}