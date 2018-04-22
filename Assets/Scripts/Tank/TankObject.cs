using System;
using svtz.Tanks.Assets.Scripts.Common;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;
using Random = UnityEngine.Random;

namespace svtz.Tanks.Assets.Scripts.Tank
{
    internal sealed class TankObject : GameObjectWrapper<TankObject>
    {
        [Serializable]
        public class Settings
        {
#pragma warning disable 0649
            public GameObject TankPrefab;
#pragma warning restore 0649
        }

        public Vector2 Position { get { return GameObject.transform.position; } }

        public TankObject(DiContainer container, Settings settings, NetworkConnection connection, Vector2 position)
            : base(container, settings.TankPrefab)
        {
            GameObject.transform.position = position;
            GameObject.transform.rotation = GetRandomQuanterion();

            NetworkServer.ReplacePlayerForConnection(connection, GameObject, 0);
        }

        public void Destroy()
        {
            NetworkServer.Destroy(GameObject);
        }


        private static Quaternion GetRandomQuanterion()
        {
            return Quaternion.Euler(0, 0, Random.Range(0, 4) * 90);
        }

        public class Factory : Factory<NetworkConnection, Vector2, TankObject> { }
    }
}
