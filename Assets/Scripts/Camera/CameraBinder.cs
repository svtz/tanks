using UnityEngine;
using UnityEngine.Networking;
using Zenject;

namespace svtz.Tanks.Assets.Scripts.Camera
{
    [RequireComponent(typeof(NetworkIdentity))]
    internal sealed class CameraBinder : NetworkBehaviour
    {
        private CameraController _cc;

        [Inject]
        private void Construct(CameraController cameraController)
        {
            _cc = cameraController;
        }

        public override void OnStartAuthority()
        {
            base.OnStartAuthority();

            _cc.BindTo(gameObject);
        }

        public override void OnStopAuthority()
        {
            base.OnStopAuthority();

            _cc.Unbind();
        }
    }
}
