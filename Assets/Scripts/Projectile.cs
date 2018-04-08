using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int Damage;

    private TeamId _id;

    public void Start()
    {
        _id = GetComponent<TeamId>();
    }

    void OnCollisionEnter2D(Collision2D collision)
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
