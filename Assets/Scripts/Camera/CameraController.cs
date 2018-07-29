using System;
using UnityEngine;

namespace svtz.Tanks.Camera
{
    [RequireComponent(typeof(PerfectPixel))]
    internal sealed class CameraController : MonoBehaviour
    {
#pragma warning disable 0649
        public float MaxCameraSpeed;
        public Vector3 Offset;
#pragma warning restore 0649

        private GameObject _followObject;
        private PerfectPixel _perfectPixel;

        private void Start()
        {
            _perfectPixel = GetComponent<PerfectPixel>();
        }

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
            transform.position = RoundToPPU(transform.position + currentMove);
        }

        private Vector3 RoundToPPU(Vector3 position)
        {
            var upp = 1 / _perfectPixel.ActualPPU;
            return new Vector3(
                Mathf.Round(position.x / upp) * upp,
                Mathf.Round(position.y / upp) * upp,
                position.z);
        }
    }
}