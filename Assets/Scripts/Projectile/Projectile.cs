using svtz.Tanks.Common;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;

namespace svtz.Tanks.Projectile
{
    internal sealed class Projectile : MonoBehaviour
    {
#pragma warning disable 0649
        public int Damage;
        public float TTL;
        public float Speed;
#pragma warning restore 0649

        private static int _seqId = 0;

        private readonly int _id = ++_seqId;
        private Rigidbody2D _rb2D;
        private ProjectilePool _pool;
        private DelayedExecutor _delayedExecutor;

        private TeamId _teamId;
        private DelayedExecutor.ICancellable _autoDespawn;
        private bool _despawned;

        [Inject]
        public void Construct(Rigidbody2D rb2D, ProjectilePool pool, DelayedExecutor delayedExecutor)
        {
            _rb2D = rb2D;
            _pool = pool;
            _delayedExecutor = delayedExecutor;
        }

        public void Launch(Transform relativeSpawn, TeamId teamId)
        {
            _teamId = teamId;

            transform.SetParent(transform);

            transform.position = relativeSpawn.position;
            transform.rotation = relativeSpawn.rotation;

            _rb2D.velocity = transform.up * Speed;

            _despawned = false;
            _autoDespawn = _delayedExecutor.Add(TryDespawn, TTL);

            Debug.LogWarning("Запущен снаряд " + _id);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            Debug.LogWarning("Снаряд " + _id + " с чем-то столкнулся.");

            var hit = collision.gameObject;
            var teamId = hit.GetComponent<TeamId>();

            if (teamId == null || teamId.Id != _teamId.Id)
            {
                var health = hit.GetComponent<HealthBase>();
                if (health != null)
                {
                    health.TakeDamage(Damage);
                }

                Debug.LogWarning("Инициирую уничтожение снаряда " + _id + " в результате столкновения.");
                TryDespawn();
            }
        }

        private void TryDespawn()
        {
            if (!_despawned)
            {
                Debug.LogWarning("Уничтожаю снаряд " + _id);

                if (_autoDespawn != null)
                {
                    _autoDespawn.Cancel();
                    Debug.LogWarning("Автодеспавн отменён " + _id);
                }
                if (NetworkServer.active)
                {
                    _pool.Despawn(this);
                    Debug.LogWarning("Снаряд убран в пул. Уничтожение завершено.");
                }
                else
                {
                    gameObject.SetActive(false);
                    Debug.LogWarning("Снаряд деактивирован. Уничтожение завершено.");
                }
            }
            _despawned = true;
        }
    }
}
