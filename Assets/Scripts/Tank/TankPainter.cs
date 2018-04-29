using svtz.Tanks.Common;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;

namespace svtz.Tanks.Tank
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
        private SpriteRenderer _spriteRenderer;

        [Inject]
        private void Construct(
            TeamManager teamManager,
            TeamId id,
            NetworkIdentity identity,
            SpriteRenderer spriteRenderer)
        {
            _teamManager = teamManager;
            _teamId = id;
            _identity = identity;
            _spriteRenderer = spriteRenderer;
        }

        private void Start()
        {
            _spriteRenderer.color = GetColor();
            Destroy(this);
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
