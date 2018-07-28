using System;
using System.Linq;
using svtz.Tanks.Audio;
using svtz.Tanks.Common;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;

namespace svtz.Tanks.Projectile
{
    internal sealed class GaussShot : AbstractShot
    {
#pragma warning disable 0649
        public Vector2 BoxCastSize;

        public float MaxDistance;

        public LineRenderer LineRenderer;

        public float ShotDelay;
#pragma warning restore 0649

        private DelayedExecutor _delayedExecutor;
        private GaussianBurstController.Pool _burstPool;
        private float _currentDelay;
        private bool _calculated;
        private DelayedExecutor.IDelayedTask _autoDespawn;
        private GaussShotPool _pool;

        [Inject]
        public void Construct(
            DelayedExecutor delayedExecutor,
            GaussShotPool pool,
            GaussianBurstController.Pool burstPool)
        {
            _delayedExecutor = delayedExecutor;
            _burstPool = burstPool;
            _pool = pool;
        }

        protected override void OnRpcLaunch()
        {
            base.OnRpcLaunch();

            LineRenderer.positionCount = 0;
            _currentDelay = 0;
            _calculated = false;
            _autoDespawn = null;
        }

        protected override SoundEffectKind LaunchSound
        {
            get { return SoundEffectKind.GaussCharge; }
        }
        
        private void Update()
        {
            if (Owner != null)
            {
                transform.position = Owner.transform.position;
            }
            else
            {
                if (isServer)
                {
                    TryDespawn();
                }
            }

            _currentDelay += Time.deltaTime;
            if (_currentDelay > ShotDelay && _autoDespawn == null)
                _autoDespawn = _delayedExecutor.Add(TryDespawn, TTL);
        }

        private void FixedUpdate()
        {
            if (!isServer || _currentDelay < ShotDelay || _calculated)
                return;

            try
            {
                var longRangeCast = Physics2D.BoxCastAll(transform.position,
                    BoxCastSize, 
                    transform.rotation.eulerAngles.z,
                    transform.up, MaxDistance)
                    .OrderBy(hit => hit.distance);

                foreach (var hit in longRangeCast)
                {
                    if (DamageTarget(hit.centroid, hit.transform.gameObject))
                    {
                        RpcShot(transform.position, hit.centroid);
                        return;
                    }
                }
            }
            finally
            {
                _calculated = true;
            }
        }

        [ClientRpc]
        private void RpcShot(Vector2 from, Vector2 to)
        {
            LineRenderer.positionCount = 2;
            LineRenderer.SetPosition(0, from);
            LineRenderer.SetPosition(1, to);
            _burstPool.Spawn(to);

            SoundEffectsFactory.Play(to, SoundEffectKind.GaussShot,
                isLocalPlayer ? SoundEffectSource.LocalPlayer : SoundEffectSource.Environment);
        }

        protected override void DoDespawn()
        {
            _autoDespawn.Cancel();
            _pool.Despawn(this);
        }
    }
}