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

        public void BindTo(GameObject obj)
        {
            _followObject = obj;
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
            transform.position = transform.position + currentMove;
        }
    }
}