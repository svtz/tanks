using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireController : MonoBehaviour {

    public GameObject BulletPrefab;
    public Transform BulletSpawn;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
	    if (Input.GetKeyDown(KeyCode.Space))
	    {
	        Fire();
	    }
    }

    void Fire()
    {
        // Create the Bullet from the Bullet Prefab
        var bullet = (GameObject)Instantiate(
            BulletPrefab,
            BulletSpawn.position,
            BulletSpawn.rotation);

        // Add velocity to the bullet
        bullet.GetComponent<Rigidbody2D>().velocity = bullet.transform.up * 6;

        // Destroy the bullet after 2 seconds
        Destroy(bullet, 2.0f);
    }
}
