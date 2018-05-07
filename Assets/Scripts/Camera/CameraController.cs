using UnityEngine;

namespace svtz.Tanks.Camera
{
    internal sealed class CameraController : MonoBehaviour
    {
#pragma warning disable 0649
        public float MaxCameraSpeed;
        public Vector3 Offset;
#pragma warning restore 0649

        private GameObject _followObject;
        private PerfectPixel _perfectPixel;

        public void BindTo(GameObject obj)
        {
            _followObject = obj;
            _perfectPixel = GetComponent<PerfectPixel>();
        }

        public void Unbind()
        {
            _followObject = null;
        }

        private void LateUpdate()
        {
            if (_followObject == null)
                return;

            var targetPosition = _followObject.transform.position + Offset;
            var targetMove = targetPosition - transform.position;
            var currentMove = Vector3.ClampMagnitude(targetMove, MaxCameraSpeed * Time.deltaTime);
            var newPostion = transform.position + currentMove;

            var roundedX = RoundToNearestPixel(newPostion.x);
            var roundedY = RoundToNearestPixel(newPostion.y);
            transform.position = new Vector3(roundedX, roundedY) + Offset;
        }

        private float RoundToNearestPixel(float unityUnits)
        {
            var valueInPixels = unityUnits * _perfectPixel.PPU;
            valueInPixels = Mathf.Round(valueInPixels);
            var roundedUnityUnits = valueInPixels * (1 / _perfectPixel.PPU);
            return roundedUnityUnits;
        }
    }
}