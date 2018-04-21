using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using Random = UnityEngine.Random;

namespace svtz.Tanks.Assets.Scripts
{
    internal sealed class SpawnController : MonoBehaviour
    {
#pragma warning disable 0649
        public GameObject PlayerPrefab;
        public int RespawnSeconds;
#pragma warning restore 0649

        private readonly List<Vector2> _points = new List<Vector2>();
        private readonly List<GameObject> _spawnedPlayers = new List<GameObject>();

        public void Add(float x, float y)
        {
            _points.Add(new Vector2(x, y));
        }

        public void Start()
        {
            foreach (var networkConnection in NetworkServer.connections)
            {
                SpawnPlayerForConnection(networkConnection);
            }
        }

        private void SpawnPlayerForConnection(NetworkConnection networkConnection)
        {
            var player = Instantiate(PlayerPrefab, SelectSpawnPoint(), GetRandomQuanterion());
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
            Debug.Log("Destroying");
            _spawnedPlayers.Remove(playerObject);
            NetworkServer.Destroy(playerObject);
            StartCoroutine(WaitAndRespawn(connection));
        }

        private IEnumerator WaitAndRespawn(NetworkConnection connection)
        {
            yield return new WaitForSeconds(RespawnSeconds);
            if (connection.isConnected && connection.isReady)
                SpawnPlayerForConnection(connection);
        }
    }
}
