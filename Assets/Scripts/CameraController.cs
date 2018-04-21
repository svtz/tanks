using JetBrains.Annotations;
using UnityEngine;

namespace svtz.Tanks.Assets.Scripts
{
    internal sealed class CameraController : MonoBehaviour
    {
        private GameObject _followObject;
        public void StartFollow(GameObject obj)
        {
            _followObject = obj;
        }

        public void StopFollow()
        {
            _followObject = null;
        }

        private readonly Vector3 _offset = new Vector3(0, 0, -10);

        private void LateUpdate()
        {
            transform.position = _followObject.transform.position + _offset;
        }
    }
}