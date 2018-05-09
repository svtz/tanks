using svtz.Tanks.Map;
using UnityEngine;
using Zenject;

namespace svtz.Tanks.Tank
{
    internal sealed class CrawlerBelts : MonoBehaviour
    {
        [Inject]
        private void Construct(IMapSettings mapSettings, SpriteRenderer spriteRenderer)
        {
            spriteRenderer.color = mapSettings.CrawlerBeltColor;
            Destroy(this);
        }
    }
}
