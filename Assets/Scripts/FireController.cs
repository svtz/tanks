using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FireController : NetworkBehaviour
{

    public float Speed;
    public float TTL;
    public GameObject BulletPrefab;
    public Transform BulletSpawn;

    private TeamId _id;

    // Use this for initialization
    public void Start()
    {
        _id = GetComponent<TeamId>();

    }

    // Update is called once per frame
    public void Update()
    {
        if (!isLocalPlayer)
            return;

	    if (Input.GetKeyDown(KeyCode.Space))
	    {
	        var id = _id.Id;
	        CmdFire(id);
	    }
    }

    [Command]
    public void CmdFire(string teamId)
    {
        var projectile = (GameObject)Instantiate(
            BulletPrefab,
            BulletSpawn.position,
            BulletSpawn.rotation);

        // Add command
        projectile.GetComponent<TeamId>().Id = teamId;

        // Add velocity
        projectile.GetComponent<Rigidbody2D>().velocity = projectile.transform.up * Speed;

        // Spawn on the Clients
        NetworkServer.Spawn(projectile);

        // Destroy after 2 seconds
        Destroy(projectile, TTL);
    }
}
