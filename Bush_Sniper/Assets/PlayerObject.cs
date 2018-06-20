using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerObject : NetworkBehaviour
{

    public float movementSpeed = 0.5f;
    public Rigidbody player = null;
    public GameObject bullet;
    private bool allowfire;

    public float moveSpeed = 5f;

    void Start()
    {
        player = GetComponent<Rigidbody>();

        // network stuff
        ///////////////////////////////////

        if (isLocalPlayer == false)
        {
            // this object belongs to another player.
            return;
        }

        // since the PlayerObject is invisible and not part of the world
        // give me something physical to move around

        Debug.Log("PLayerObject::Start -- Spawning my own personal unit.");
        //    Instantiate(PlayerUnitPrefab);
    }

    // public GameObject PlayerUnitPrefab,

    void Update()
    {


        // hack
        if (transform.position.z != 0)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, 0); // stay on playground
        }

        if (transform.rotation.x != 0 || transform.rotation.y != 0 || transform.rotation.z != 0)
        {
            transform.rotation = Quaternion.identity;
        }

        if (player.velocity.x != 0 || player.velocity.y != 0 || player.velocity.z != 0)
        {
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
        if (Input.GetKey(KeyCode.Space))
        {
            // mousePosition relative to scene NOT to camera (this would be Input.mousePosition ;) )
            Vector3 mousePos2D = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Debug.Log("mousePos2D: " + mousePos2D);

            // playerPosition relative to scene
            Vector3 playerPos2D = new Vector3(player.position.x, player.position.y, 0);
            Debug.Log("playerPos2D: " + playerPos2D);

            // Create a ray from the transform position along the transform's z-axis
            Vector3 heading = mousePos2D - playerPos2D;
            var distance = heading.magnitude;
            var direction = heading / distance;
            Ray ray = new Ray(playerPos2D, direction);

            GameObject bulletClone = Instantiate(bullet, playerPos2D, Quaternion.identity);
            bulletClone.transform.position = playerPos2D;
            bulletClone.transform.LookAt(heading);

        }
    }
}