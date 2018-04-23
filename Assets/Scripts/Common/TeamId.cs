using UnityEngine.Networking;
using Zenject;

namespace svtz.Tanks.Common
{
    internal sealed class TeamId : NetworkBehaviour
    {
        [SyncVar]
        private string _id;

        private TeamManager _teamManager;

        [Inject]
        public void Construct(TeamManager teamManager)
        {
            _teamManager = teamManager;
        }

        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public override void OnStartServer()
        {
            if (connectionToClient != null)
                _id = _teamManager.GetTeamForConnection(connectionToClient);
        }
    }
}
