using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerObject : NetworkBehaviour {

    public float movementSpeed = 0.5f;
    public Rigidbody player = null;
    public GameObject test;

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

        // bullet spawninig
        if(Input.GetKey(KeyCode.Space)) {
            Vector3 shootDirection;

            /* // Gets a vector that points from the player's position to the target's.
var heading = target.position - player.position;

As well as pointing in the direction of the target object, this vector’s magnitude is equal to the distance between the two positions. It is common to need a normalized vector giving the direction to the target and also the distance to the target (say for directing a projectile). The distance between the objects is equal to the magnitude of the heading vector and this vector can be normalized by dividing it by its magnitude:-

var distance = heading.magnitude;
var direction = heading / distance; // This is now the normalized direction.

            */

            Vector3 playerPosNormalized = player.position.normalized;
            Vector3 mousePosNormalized = Input.mousePosition.normalized;

            Vector3 heading = playerPosNormalized - mousePosNormalized;
            heading.Normalize();
            float distance = heading.magnitude;
            Vector3 direction = heading / distance;

            // Creates the bullet locally
            GameObject bullet = (GameObject)Instantiate(
                                    test,
                player.position , Quaternion.Euler(direction));

            // Adds velocity to the bullet
            bullet.GetComponent<Rigidbody>().velocity = bullet.transform.up * moveSpeed;
        }
       }
}
