using System;
using UnityEngine;
using UnityEngine.Networking;

namespace svtz.Tanks.Assets.Scripts.Common
{
    internal sealed class TeamId : NetworkBehaviour
    {
        private const string OnTeamIdCreatedMethod = "OnTeamIdCreated";

        public static string LocalPlayerTeamId { get; private set; }

        [SyncVar]
        private string _id;

        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public override void OnStartServer()
        {
            if (string.IsNullOrEmpty(_id))
                _id = Guid.NewGuid().ToString();
        }

        public override void OnStartClient()
        {
            BroadcastMessage(OnTeamIdCreatedMethod, false, SendMessageOptions.DontRequireReceiver);
        }

        private static GameObject GetRoot(GameObject source)
        {
            var transformParent = source.transform.parent;
            if (transformParent == null)
                return source;

            return GetRoot(transformParent.gameObject);
        }

        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();

            LocalPlayerTeamId = _id;

            var root = GetRoot(gameObject);
            foreach (var rootGameObject in gameObject.scene.GetRootGameObjects())
            {
                rootGameObject.BroadcastMessage(OnTeamIdCreatedMethod, rootGameObject == root, SendMessageOptions.DontRequireReceiver);
            }
        }
    }
}
