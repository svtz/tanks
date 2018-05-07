using System;
using UnityEngine;
using Zenject;

namespace svtz.Tanks.Map
{
    internal sealed class Background : MonoBehaviour
    {
        private IMapSettings _mapSettings;
        private SpriteRenderer _spriteRenderer;

        [Inject]
        private void Construct(IMapSettings mapSettings, SpriteRenderer spriteRenderer)
        {
            _mapSettings = mapSettings;
            _spriteRenderer = spriteRenderer;
            _mapSettings.Updated += Reinit;
            Reinit(null, null);
        }

        private void Reinit(object sender, EventArgs args)
        {
            _spriteRenderer.size = new Vector2(_mapSettings.BackgroundWidth, _mapSettings.BackgroundHeight);
            _spriteRenderer.color = _mapSettings.BackgroundColor;
        }

        private void OnDestroy()
        {
            _mapSettings.Updated -= Reinit;
        }
    }
}
