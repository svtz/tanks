using svtz.Tanks.Assets.Scripts.Common;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;

namespace svtz.Tanks.Assets.Scripts.Tank
{
    public class TankPainter : MonoBehaviour
    {
#pragma warning disable 0649
        public Color PlayerColor;
        public Color EnemyColor;
        public Color AllyColor;
#pragma warning restore 0649

        private TeamId _teamId;
        private NetworkIdentity _identity;
        private TeamManager _teamManager;
        private SpriteRenderer _renderer;

        [Inject]
        private void Construct(TeamManager teamManager)
        {
            _teamManager = teamManager;
        }

        private void Start()
        {
            //todo https://github.com/modesttree/Zenject/issues/275 почему-то заинжектить не удалось
            _identity = GetComponent<NetworkIdentity>() ?? GetComponentInParent<NetworkIdentity>(); ;
            _teamId = GetComponent<TeamId>() ?? GetComponentInParent<TeamId>();
            _renderer = GetComponent<SpriteRenderer>();

            _renderer.color = GetColor();
        }

        private Color GetColor()
        {
            if (_identity.isLocalPlayer)
                return PlayerColor;

            var teamId = _teamId.Id;
            return _teamManager.IsAlly(teamId) ? AllyColor : EnemyColor;
        }
    }
}
