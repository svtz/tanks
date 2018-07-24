using svtz.Tanks.Map;
using UnityEngine;
using Zenject;

namespace svtz.Tanks.Tank
{
    internal sealed class CrawlerBeltsController : MonoBehaviour
    {
        [Inject]
        private void Construct(IMapSettings mapSettings, SpriteRenderer spriteRenderer)
        {
            spriteRenderer.color = mapSettings.CrawlerBeltColor;
        }

        private bool _currentlyEnabled;

        public void SetAnimationState(bool enableAnimation)
        {
            if (enableAnimation ^ _currentlyEnabled)
            {
                GetComponent<Animator>().SetBool("Moving", enableAnimation);
                _currentlyEnabled = enableAnimation;
            }
        }
    }
}
