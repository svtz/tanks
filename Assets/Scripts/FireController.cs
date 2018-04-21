using System.Collections;
using svtz.Tanks.Assets.Scripts.Common;
using UnityEngine;
using UnityEngine.Networking;

namespace svtz.Tanks.Assets.Scripts
{
    internal sealed class FireController : NetworkBehaviour
    {
#pragma warning disable 0649
        public float Speed;
        public float TTL;
        public float Cooldown;
        public GameObject BulletPrefab;
        public Transform BulletSpawn;
#pragma warning restore 0649

        private TeamId _id;

        [SyncVar]
        private bool _canFire = true;

        // Use this for initialization
        private void Start()
        {
            _id = GetComponent<TeamId>();
        }

        // Update is called once per frame
        private void Update()
        {
            if (!isLocalPlayer)
                return;

            if (_canFire && Input.GetKey(KeyCode.Space))
            {
                var id = _id.Id;
                CmdFire(id);
            }
        }

        [Command]
        private void CmdFire(string teamId)
        {
            if (!_canFire)
                return;

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
            StartCoroutine(DoCooldown());
        }

        private IEnumerator DoCooldown()
        {
            if (!isServer) yield break;

            _canFire = false;
            yield return new WaitForSeconds(Cooldown);
            _canFire = true;
        }
    }
}
