using svtz.Tanks.Common;
using UnityEngine;

namespace svtz.Tanks
{
    internal sealed class Projectile : MonoBehaviour
    {
#pragma warning disable 0649
        public int Damage;
#pragma warning restore 0649

        private TeamId _id;

        private void Start()
        {
            _id = GetComponent<TeamId>();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            var hit = collision.gameObject;
            var teamId = hit.GetComponent<TeamId>();

            if (teamId == null || teamId.Id != _id.Id)
            {
                var health = hit.GetComponent<HealthBase>();
                if (health != null)
                {
                    health.TakeDamage(Damage);
                }

                Destroy(gameObject);
            }
        }
    }
}
