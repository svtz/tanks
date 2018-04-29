using svtz.Tanks.Common;
using UnityEngine;
using Zenject;

namespace svtz.Tanks.Projectile
{
    internal sealed class Projectile : MonoBehaviour
    {
#pragma warning disable 0649
        public int Damage;
#pragma warning restore 0649

        private TeamId _teamId;
        private Rigidbody2D _rb2d;

        [Inject]
        public void Construct(Rigidbody2D rb2d)
        {
            _rb2d = rb2d;
        }

        public void Launch(Transform relativeSpawn, float velocity, TeamId teamId)
        {
            _teamId = teamId;

            transform.SetParent(transform);

            transform.position = relativeSpawn.position;
            transform.rotation = relativeSpawn.rotation;

            _rb2d.velocity = transform.up * velocity;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            var hit = collision.gameObject;
            var teamId = hit.GetComponent<TeamId>();

            if (teamId == null || teamId.Id != _teamId.Id)
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
