using UnityEngine;
using UnityEngine.Networking;

namespace svtz.Tanks.Map
{
    internal sealed class WallHealth : NetworkBehaviour
    {
        private int _childCount;

        private void Start()
        {
            _childCount = transform.childCount;
        }

        public void DestroySegment(GameObject segment)
        {
            Debug.Assert(isServer);
            Debug.Assert(segment.transform.parent == transform);

            RpcDestroyChild(segment.name);
        }

        [ClientRpc]
        private void RpcDestroyChild(string childName)
        {
            var child = transform.Find(childName).gameObject;

            _childCount--;
            if (_childCount == 0)
            {
                Destroy(gameObject);
            }
            else
            {
                Destroy(child);
            }
        }
    }
}