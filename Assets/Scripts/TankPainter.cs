using svtz.Tanks.Assets.Scripts.Common;
using UnityEngine;

namespace svtz.Tanks.Assets.Scripts
{
    public class TankPainter : MonoBehaviour
    {
#pragma warning disable 0649
        public Color PlayerColor;
        public Color EnemyColor;
        public Color AllyColor;
#pragma warning restore 0649

        private void OnTeamIdCreated(bool isLocal)
        {
            PaintWithColor(isLocal ? PlayerColor : GetColor());
        }

        private Color GetColor()
        {
            var teamId = (GetComponent<TeamId>() ?? GetComponentInParent<TeamId>()).Id;
            return teamId == TeamId.LocalPlayerTeamId ? AllyColor : EnemyColor;
        }

        private void PaintWithColor(Color color)
        {
            var sprite = GetComponent<SpriteRenderer>();
            sprite.color = color;
        }
    }
}
