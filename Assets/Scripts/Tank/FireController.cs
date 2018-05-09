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

        private ProjectilePool _pool;
        private DelayedExecutor _delayedExecutor;
        private InputManager _input;

        [Inject]
        private void Construct(ProjectilePool pool, DelayedExecutor delayedExecutor,
            InputManager input)
        {
            _pool = pool;
            _delayedExecutor = delayedExecutor;
            _input = input;
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
                var doFire = _input.Fire();
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
                    projectile.Launch(ProjectileSpawn, gameObject);
                    
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
