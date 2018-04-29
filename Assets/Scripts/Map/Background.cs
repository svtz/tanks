using UnityEngine;
using UnityEngine.Networking;

namespace svtz.Tanks.Map
{
    internal sealed class Background : NetworkBehaviour
    {
        [SyncVar]
        private int _width;
        [SyncVar]
        private int _height;

        private bool _initialized;

        private void Start()
        {
            if (isServer && !_initialized)
                Debug.LogError("размер фона не был установлен!");
            InitBackground();
        }

        private void InitBackground()
        {
            GetComponent<SpriteRenderer>().size = new Vector2(_width, _height);
            Destroy(this);
        }

        public void SetSize(int width, int height)
        {
            _width = width;
            _height = height;

            _initialized = true;
        }
    }
}
