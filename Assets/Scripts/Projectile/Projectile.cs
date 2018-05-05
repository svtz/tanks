using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using svtz.Tanks.Common;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Networking;
using Zenject;
using Debug = UnityEngine.Debug;

namespace svtz.Tanks.Projectile
{
    internal sealed class Projectile : NetworkBehaviour
    {
#pragma warning disable 0649
        public int Damage;
        public float TTL;
        public float Speed;

        public float CastWidth;
        public float CastDistance;
#pragma warning restore 0649

        private Rigidbody2D _rb2D;
        private ProjectilePool _pool;
        private DelayedExecutor _delayedExecutor;

        private GameObject _owner;
        private DelayedExecutor.ICancellable _autoDespawn;
        private bool _despawned;

        [Inject]
        public void Construct(Rigidbody2D rb2D, ProjectilePool pool, DelayedExecutor delayedExecutor)
        {
            _rb2D = rb2D;
            _pool = pool;
            _delayedExecutor = delayedExecutor;
        }

        [ClientRpc]
        private void RpcLaunch(Vector2 position, Quaternion rotation, GameObject owner)
        {
            _owner = owner;

            transform.position = position;
            transform.rotation = rotation;
            _rb2D.velocity = transform.up * Speed;

            _despawned = false; 
            _autoDespawn = _delayedExecutor.Add(TryDespawn, TTL);
        }

        public void Launch(Transform spawn, GameObject owner)
        {
            Assert.IsTrue(isServer);
            RpcLaunch(spawn.position, spawn.rotation, owner);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject == _owner)
                return;
            
            var contact = new Vector2();

            foreach (var collisionContact in collision.contacts)
            {
                contact += collisionContact.point;
            }
            contact /= collision.contacts.Length;

            Debug.DrawLine(_owner.transform.position, contact, Color.yellow, 2);

            Vector2 direction = transform.up;
            const float offset = 0.01f;

            ExtDebug.DrawBoxCastBox(
                contact,
                new Vector2(CastWidth / 2, offset / 2),
                transform.rotation,
                direction,
                CastDistance,
                Color.magenta);

            var cast = Physics2D.BoxCastAll(
                    contact,
                    new Vector2(CastWidth, offset),
                    angle: transform.rotation.eulerAngles.z,
                    direction: direction,
                    distance: CastDistance)
                .Select(h => h.transform)
                .Where(h => h != transform && h != _owner.transform)
                .Distinct();


            var shouldDespawn = false;
            foreach (var hit in cast)
            {
                var target = hit.GetComponent<AbstractProjectileTarget>();
                if (target != null)
                {
                    if (isServer)
                    {
                        target.TakeDamage(Damage, _owner);
                    }
                    shouldDespawn = true;
                }
            }

            if (shouldDespawn)
            {
                TryDespawn();
            }
            else
            {
                Debug.Break();
            }
        }

        public void TryDespawn()
        {
            if (!_despawned)
            {
                _autoDespawn.Cancel();
                _pool.Despawn(this);
            }

            _despawned = true;
        }
    }


}
