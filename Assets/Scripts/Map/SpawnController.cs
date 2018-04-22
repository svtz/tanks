using System;
using System.Collections.Generic;
using System.Linq;
using svtz.Tanks.Assets.Scripts.Common;
using svtz.Tanks.Assets.Scripts.Tank;
using UnityEngine;
using UnityEngine.Networking;
using Random = UnityEngine.Random;

namespace svtz.Tanks.Assets.Scripts.Map
{
    internal sealed class SpawnController
    {
        [Serializable]
        public class Settings
        {
#pragma warning disable 0649
            public int RespawnSeconds;
#pragma warning restore 0649
        }

        private readonly Settings _settings;
        private readonly DelayedExecutor _delayedExecutor;
        private readonly TankFactory _tankFactory;

        public SpawnController(Settings settings, DelayedExecutor delayedExecutor, TankFactory tankFactory)
        {
            _settings = settings;
            _delayedExecutor = delayedExecutor;
            _tankFactory = tankFactory;
        }

        private readonly List<Vector2> _points = new List<Vector2>();
        private readonly List<GameObject> _spawnedPlayers = new List<GameObject>();

        public void AddSpawnPoint(float x, float y)
        {
            _points.Add(new Vector2(x, y));
        }

        public void SpawnAllPlayers()
        {
            foreach (var networkConnection in NetworkServer.connections)
            {
                SpawnPlayerForConnection(networkConnection);
            }
        }

        private void SpawnPlayerForConnection(NetworkConnection networkConnection)
        {
            var player = _tankFactory.Create(SelectSpawnPoint());
            _spawnedPlayers.Add(player);
            NetworkServer.ReplacePlayerForConnection(networkConnection, player, 0);
        }

        private Vector2 SelectSpawnPoint()
        {
            if (!_spawnedPlayers.Any())
                return _points[Random.Range(0, _points.Count)];

            return _points
                .Select(s => new
                {
                    SpawnPoint = s,
                    MinimalDistanceToPlayer = _spawnedPlayers.Min(p => (s - (Vector2)p.transform.position).magnitude)
                })
                .OrderByDescending(s => s.MinimalDistanceToPlayer)
                .First()
                .SpawnPoint;
        }


        public void DestroyAndRespawn(NetworkConnection connection, GameObject playerObject)
        {
            _spawnedPlayers.Remove(playerObject);
            NetworkServer.Destroy(playerObject);
            _delayedExecutor.Add(() => Respawn(connection), _settings.RespawnSeconds);
        }

        private void Respawn(NetworkConnection connection)
        {
            if (connection.isReady)
                SpawnPlayerForConnection(connection);
        }
    }
}
