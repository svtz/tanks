using System;
using System.Collections.Generic;
using System.Linq;
using svtz.Tanks.Assets.Scripts.Common;
using UnityEngine;
using UnityEngine.Networking;
using Random = UnityEngine.Random;
using UnityObject = UnityEngine.Object;

namespace svtz.Tanks.Assets.Scripts.Map
{
    internal sealed class SpawnController
    {
        [Serializable]
        public class Settings
        {
#pragma warning disable 0649
            public GameObject PlayerPrefab;
            public int RespawnSeconds;
#pragma warning restore 0649
        }

        private readonly Settings _settings;
        private readonly DelayedExecutor _delayedExecutor;

        public SpawnController(Settings settings, DelayedExecutor delayedExecutor)
        {
            _settings = settings;
            _delayedExecutor = delayedExecutor;
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
            if (!networkConnection.isReady)
                return;

            var player = UnityObject.Instantiate(_settings.PlayerPrefab, SelectSpawnPoint(), GetRandomQuanterion());
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

        private Quaternion GetRandomQuanterion()
        {
            return Quaternion.Euler(0, 0, Random.Range(0, 4) * 90);
        }

        public void DestroyAndRespawn(NetworkConnection connection, GameObject playerObject)
        {
            _spawnedPlayers.Remove(playerObject);
            NetworkServer.Destroy(playerObject);
            _delayedExecutor.Add(() => SpawnPlayerForConnection(connection), _settings.RespawnSeconds);
        }
    }
}
