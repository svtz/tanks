using System;
using UnityEngine.Networking;

namespace svtz.Tanks.Assets.Scripts.Common
{
    internal sealed class TeamId : NetworkBehaviour
    {
        [SyncVar]
        private string _id;

        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }

        // Use this for initialization
        private void Start()
        {
            if (string.IsNullOrEmpty(_id))
                _id = Guid.NewGuid().ToString();
        }
    }
}
