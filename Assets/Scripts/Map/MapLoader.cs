using svtz.Tanks.Tank;
using UnityEngine;
using Zenject;

namespace svtz.Tanks.Map
{
    internal sealed class MapLoader : MonoBehaviour
    {
        private MapCreator _mapCreator;
        private TankSpawner _tankSpawner;

        [Inject]
        private void Construct(MapCreator mapCreator, TankSpawner tankSpawner)
        {
            _mapCreator = mapCreator;
            _tankSpawner = tankSpawner;
        }

        private void Start()
        {
            _mapCreator.Create();
            _tankSpawner.SpawnAllPlayers();

            Destroy(gameObject);
        }
    }
}