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
        public float Cooldown;
        public Transform ProjectileSpawn;
#pragma warning restore 0649

        private TeamId _teamId;
        private ProjectilePool _pool;
        private DelayedExecutor _delayedExecutor;

        [Inject]
        private void Construct(TeamId teamId, ProjectilePool pool, DelayedExecutor delayedExecutor)
        {
            _teamId = teamId;
            _pool = pool;
            _delayedExecutor = delayedExecutor;
        }


        private bool _canFire = true;

        [SyncVar]
        private bool _isFiring = false;

        // Update is called once per frame
        private void Update()
        {
            if (isLocalPlayer)
            {
                // клиент: сообщаем на сервер положение кнопки
                var doFire = Input.GetKey(KeyCode.Space);
                if (doFire ^ _isFiring)
                {
                    CmdFire(doFire);
                }
            }

            if (isServer)
            {
                // сервер: стреляем
                if (_isFiring && _canFire)
                {
                    var projectile = _pool.Spawn();
                    projectile.Launch(ProjectileSpawn, _teamId);
                    
                    // кулдаун
                    _canFire = false;
                    _delayedExecutor.Add(() => _canFire = true, Cooldown);
                }
            }
        }

        [Command]
        private void CmdFire(bool value)
        {
            _isFiring = value;
        }
    }
}
