using System;
using System.Collections.Generic;
using System.Linq;
using svtz.Tanks.Common;
using UnityEngine;
using UnityEngine.Networking;
using Random = UnityEngine.Random;
using UnityObject = UnityEngine.Object;

namespace svtz.Tanks.Tank
{
    internal sealed class TankSpawner
    {
        [Serializable]
        public class Settings
        {
#pragma warning disable 0649
            public int RespawnSeconds;
            public GameObject TankPrefab;
#pragma warning restore 0649
        }

        private readonly Settings _settings;
        private readonly DelayedExecutor _delayedExecutor;
        private readonly RespawningSignal.ServerToClient _respawningSignal;

        public TankSpawner(Settings settings, DelayedExecutor delayedExecutor,
            RespawningSignal.ServerToClient respawningSignal)
        {
            _settings = settings;
            _delayedExecutor = delayedExecutor;
            _respawningSignal = respawningSignal;
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
            var player = UnityObject.Instantiate(_settings.TankPrefab);

            var transform = player.transform;
            transform.position = SelectSpawnPoint();
            transform.rotation = GetRandomQuanterion();

            NetworkServer.ReplacePlayerForConnection(networkConnection, player, 0);
            _spawnedPlayers.Add(player);
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
            _respawningSignal.FireOnClient(connection, new RespawningSignal.Msg { Time = _settings.RespawnSeconds });
            _delayedExecutor.Add(() => Respawn(connection), _settings.RespawnSeconds);
        }

        private void Respawn(NetworkConnection connection)
        {
            if (connection.isReady)
                SpawnPlayerForConnection(connection);
        }

        private static Quaternion GetRandomQuanterion()
        {
            return Quaternion.Euler(0, 0, Random.Range(0, 4) * 90);
        }

    }
}
