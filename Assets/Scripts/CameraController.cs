using JetBrains.Annotations;
using UnityEngine;

namespace svtz.Tanks.Assets.Scripts
{
    internal sealed class CameraController : MonoBehaviour
    {
#pragma warning disable 0649
        public GameObject Player;
#pragma warning restore 0649

        private Vector3 _offset;

        private void Start()
        {
            //_offset = transform.position - Player.transform.position;
        }

        private void LateUpdate()
        {
            //transform.position = Player.transform.position + _offset;
        }
    }
}