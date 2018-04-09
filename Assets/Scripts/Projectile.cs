﻿using svtz.Tanks.Assets.Scripts.Common;
using UnityEngine;

namespace svtz.Tanks.Assets.Scripts
{
    internal sealed class Projectile : MonoBehaviour
    {
        public int Damage;

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
                var health = hit.GetComponent<Health>();
                if (health != null)
                {
                    health.TakeDamage(Damage);
                }

                Destroy(gameObject);
            }
        }
    }
}
