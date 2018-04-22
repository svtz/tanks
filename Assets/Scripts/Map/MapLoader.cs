using UnityEngine.Networking;
using Zenject;

namespace svtz.Tanks.Assets.Scripts.Map
{
    internal sealed class MapLoader : NetworkBehaviour
    {
        private MapCreator _mapCreator;
        private SpawnController _spawnController;

        [Inject]
        private void Construct(MapCreator mapCreator, SpawnController spawnController)
        {
            _mapCreator = mapCreator;
            _spawnController = spawnController;
        }

        // Use this for initialization
        public override void OnStartServer()
        {
            _mapCreator.Create();
        }

        private void Start()
        {
            _spawnController.SpawnAllPlayers();
        }
    }
}