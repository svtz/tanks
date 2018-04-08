using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private TeamId _id;

    public void Start()
    {
        _id = GetComponent<TeamId>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        var teamId = collision.gameObject.GetComponent<TeamId>();

        if (teamId == null || teamId.Id != _id.Id)
            Destroy(gameObject);
    }
}
