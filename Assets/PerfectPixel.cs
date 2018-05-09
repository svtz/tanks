using UnityEngine;

namespace svtz.Tanks
{
    public class PerfectPixel : MonoBehaviour
    {
#pragma warning disable 0649
        public float PPU;
#pragma warning restore 0649

        private int _currentHeight;
        private UnityEngine.Camera _camera;

        private void Start()
        {
            _camera = GetComponent<UnityEngine.Camera>();
        }

        private void Update()
        {
            if (Screen.height == _currentHeight)
            {
                return;
            }

            _currentHeight = Screen.height;
            var idealSize = _currentHeight / PPU * 0.5f;
            _camera.orthographicSize = idealSize;
        }
    }
}
