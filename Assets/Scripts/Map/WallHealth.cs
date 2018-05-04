using System;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;

namespace svtz.Tanks.Map
{
    internal sealed class WallHealth : NetworkBehaviour
    {
        private MapObjectsManager _mapObjectsManager;
        private int _childCount;

        [Inject]
        private void Construct(MapObjectsManager mapObjectsManager)
        {
            _mapObjectsManager = mapObjectsManager;
            _childCount = transform.childCount;
        }

        public void DestroySegment(GameObject segment)
        {
            Debug.Assert(isServer);
            Debug.Assert(segment.transform.parent == transform);

            RpcDestroyChild(segment.name);

            _childCount--;
            if (_childCount == 0)
                _mapObjectsManager.Remove(gameObject);
        }

        [ClientRpc]
        private void RpcDestroyChild(string childName)
        {
            var child = transform.Find(childName).gameObject;
            Destroy(child);
        }
    }
}