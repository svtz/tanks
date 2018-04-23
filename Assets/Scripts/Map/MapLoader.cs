using UnityEngine.Networking;
using Zenject;

namespace svtz.Tanks.Map
{
    internal sealed class MapLoader : NetworkBehaviour
    {
        private MapCreator _mapCreator;
        private TankSpawner _tankSpawner;

        [Inject]
        private void Construct(MapCreator mapCreator, TankSpawner tankSpawner)
        {
            _mapCreator = mapCreator;
            _tankSpawner = tankSpawner;
        }

        // Use this for initialization
        public override void OnStartServer()
        {
            _mapCreator.Create();
        }

        private void Start()
        {
            _tankSpawner.SpawnAllPlayers();
        }
    }
}