using UnityEngine;

namespace svtz.Tanks
{
    [RequireComponent(typeof(UnityEngine.Camera))]
    public class PerfectPixel : MonoBehaviour
    {
#pragma warning disable 0649
        public float PPU;
        public float ViewRange;
#pragma warning restore 0649

        private int _currentHeight;
        private UnityEngine.Camera _camera;
        private UnityEngine.Camera _displacementEffectsCamera;

        public float ActualPPU { get; private set; }

        private void Start()
        {
            _camera = GetComponent<UnityEngine.Camera>();
            _displacementEffectsCamera = transform.GetChild(0).gameObject.GetComponent<UnityEngine.Camera>();
        }

        private void Update()
        {
            if (Screen.height == _currentHeight)
            {
                return;
            }

            _currentHeight = Screen.height;

            var idealSize = ViewRange * PPU;
            ActualPPU = Mathf.Floor(_currentHeight / idealSize) * PPU;
            _camera.orthographicSize = _currentHeight / ActualPPU / 2;
            _displacementEffectsCamera.orthographicSize = _camera.orthographicSize;
        }
    }
}
