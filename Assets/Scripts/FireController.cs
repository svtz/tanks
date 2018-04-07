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

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (!isLocalPlayer)
            return;

	    if (Input.GetKeyDown(KeyCode.Space))
	    {
	        CmdFire();
	    }
    }

    [Command]
    public void CmdFire()
    {
        var projectile = (GameObject)Instantiate(
            BulletPrefab,
            BulletSpawn.position,
            BulletSpawn.rotation);

        // Add velocity
        projectile.GetComponent<Rigidbody2D>().velocity = projectile.transform.up * Speed;

        // Spawn on the Clients
        NetworkServer.Spawn(projectile);

        // Destroy after 2 seconds
        Destroy(projectile, TTL);
    }
}
