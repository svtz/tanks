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
        public Transform ProjectileSpawn;
#pragma warning restore 0649

        private InputManager _input;

        private IGun _gun;

        public void SetGun(IGun gun)
        {
            Debug.Assert(gun != null);
            _gun = gun;
        }

        [Inject]
        private void Construct(
            GaussShotPool gaussPool,
            InputManager input,
            BulletGun defaultGun)
        {
            _input = input;
            _gun = defaultGun;
        }

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
                if (_isFiring && _gun.CanFire)
                {
                    _gun.Fire(ProjectileSpawn, gameObject);
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
