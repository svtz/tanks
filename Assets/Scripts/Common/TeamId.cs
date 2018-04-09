using System;
using UnityEngine.Networking;

namespace svtz.Tanks.Assets.Scripts.Common
{
    public class TeamId : NetworkBehaviour
    {
        [SyncVar]
        private string _id;

        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }

        // Use this for initialization
        void Start ()
        {
            if (string.IsNullOrEmpty(_id))
                _id = Guid.NewGuid().ToString();
        }
	
        // Update is called once per frame
        void Update () {
		
        }
    }
}
