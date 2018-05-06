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
        [SyncVar]
        private Color _color;

        private bool _initialized;

        private void Start()
        {
            if (isServer && !_initialized)
                Debug.LogError("размер фона не был установлен!");
            InitBackground();
        }

        private void InitBackground()
        {
            var render = GetComponent<SpriteRenderer>();
            render.size = new Vector2(_width, _height);
            render.color = _color;
            Destroy(this);
        }

        public void SetSize(int width, int height, Color color)
        {
            _width = width;
            _height = height;
            _color = color;

            _initialized = true;
        }
    }
}
