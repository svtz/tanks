using System;
using UnityEngine;

namespace svtz.Tanks.Map
{
    internal interface IMapSettings
    {
        float BackgroundWidth { get; }
        float BackgroundHeight { get; }
        Color BackgroundColor { get; }
        Color CrawlerBeltColor { get; }

        event EventHandler Updated;
    }
}