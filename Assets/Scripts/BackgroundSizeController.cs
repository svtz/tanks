using System;
using UnityEngine;
using UnityEngine.Networking;

namespace svtz.Tanks.Assets.Scripts
{
    internal sealed class BackgroundSizeController : NetworkBehaviour
    {
        [SyncVar]
        private int _width;
        [SyncVar]
        private int _height;

        public override void OnStartClient()
        {
            InitBackground();
        }

        private void InitBackground()
        {
            GetComponent<SpriteRenderer>().size = new Vector2(_width * 2, _height * 2);
        }

        public void SetSize(int width, int height)
        {
            if (!isServer)
                return;

            _width = width;
            _height = height;

            InitBackground();
        }
    }
}
