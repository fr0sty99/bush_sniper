using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerObject : NetworkBehaviour {

    public float movementSpeed = 0.5f;
    public Rigidbody player = null;

    public float moveSpeed = 5f;

	void Start () {
        player = GetComponent<Rigidbody>();

        // network stuff
        ///////////////////////////////////

        if(isLocalPlayer == false) {
            // this object belongs to another player.
            return;
        }

        // since the PlayerObject is invisible and not part of the world
        // give me something physical to move around

        Debug.Log("PLayerObject::Start -- Spawning my own personal unit.");
    //    Instantiate(PlayerUnitPrefab);
	}

   // public GameObject PlayerUnitPrefab,
	
	void Update () {
        // hack
        if(transform.position.z != 0) {
            transform.position = new Vector3(transform.position.x, transform.position.y, 0); // stay on playground
        }

        if(transform.rotation.x != 0 || transform.rotation.y != 0 || transform.rotation.z != 0) {
            transform.rotation = Quaternion.identity;
        }

        if(player.velocity.x != 0 || player.velocity.y != 0 || player.velocity.z != 0) {
            player.velocity = Vector3.zero;
        }

        // end of hack



        // movement

        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(Vector3.down * moveSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
        }
	}
}
