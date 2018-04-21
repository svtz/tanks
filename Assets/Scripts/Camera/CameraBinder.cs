using UnityEngine;
using UnityEngine.Networking;

namespace svtz.Tanks.Assets.Scripts.Camera
{
    [RequireComponent(typeof(NetworkIdentity))]
    internal sealed class CameraBinder : NetworkBehaviour
    {
        private CameraController _cc;

        public override void OnStartAuthority()
        {
            base.OnStartAuthority();

            _cc = FindObjectOfType<CameraController>();
            _cc.BindTo(gameObject);
        }

        public override void OnStopAuthority()
        {
            base.OnStopAuthority();

            _cc.Unbind();
        }
    }
}
