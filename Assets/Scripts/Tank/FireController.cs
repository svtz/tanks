using System.Collections;
using svtz.Tanks.Common;
using svtz.Tanks.Projectile;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;

namespace svtz.Tanks.Tank
{
    [RequireComponent(typeof(TeamId))]
    internal sealed class FireController : NetworkBehaviour
    {
#pragma warning disable 0649
        public float Speed;
        public float TTL;
        public float Cooldown;
        public Transform BulletSpawn;
#pragma warning restore 0649

        private TeamId _id;
        private ProjectilePool _pool;
        private DelayedExecutor _delayedExecutor;

        [SyncVar]
        private bool _canFire = true;

        [Inject]
        private void Construct(TeamId teamId, ProjectilePool pool, DelayedExecutor delayedExecutor)
        {
            _id = teamId;
            _pool = pool;
            _delayedExecutor = delayedExecutor;
        }

        // Update is called once per frame
        private void Update()
        {
            if (!isLocalPlayer)
                return;

            if (_canFire && Input.GetKey(KeyCode.Space))
            {
                // на клиенте говорим, что больше стрелять не можем,
                // чтобы не плодить лишних сообщений
                if (!isServer)
                    _canFire = false;

                var id = _id.Id;
                CmdFire(id);
            }
        }

        [Command]
        private void CmdFire(string teamId)
        {
            // серверная проверка возможности стрельбы
            if (!_canFire)
                return;

            _canFire = false;

            var projectile = _pool.Spawn();
            projectile.Launch(BulletSpawn, Speed, _id);
            NetworkServer.Spawn(projectile.gameObject);

            // Destroy after 2 seconds
            Destroy(projectile, TTL);
            _delayedExecutor.Add(() => _canFire = true, Cooldown);
        }
    }
}
