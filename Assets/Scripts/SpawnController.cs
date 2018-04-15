using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace svtz.Tanks.Assets.Scripts
{
    internal sealed class SpawnController : NetworkBehaviour
    {
        private readonly List<Vector2> _points = new List<Vector2>();

        public void Add(float x, float y)
        {
            _points.Add(new Vector2(x, y));
        }

        public void Start()
        {
            foreach (var networkConnection in Network.connections)
            {
                networkConnection.
                NetworkServer.SpawnWithClientAuthority()
            }
        }
    }
}
