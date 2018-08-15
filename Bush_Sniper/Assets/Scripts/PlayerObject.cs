using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerObject : MonoBehaviour
{
    public bool moving = false;
    public Rigidbody2D player = null;
    public GameObject bullet;
    public GameObject gun;
    public float fireRate;
    private float nextFire;
    private LinkedList<GameObject> bulletList = new LinkedList<GameObject>();
    Vector3 currentPlayerPos;
    Vector3 currentMousePos;
    public float moveSpeed = 5f;
    public int shootingDistance;
    public float bulletSpeed = 3f;
    public float bulletSpawnDistance = 0.6f;
    public float gunSpawnDistance = 0.5f;

    void Start()
    {
        player = GetComponent<Rigidbody2D>();

        /*
        Debug.Log("before calling generateIslands");
        generateIslands();
    */
    }

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

        if (player.velocity.x != 0 || player.velocity.y != 0)
        {
            player.velocity = Vector3.zero;
        }

        // end of hack



        // movement
        if (Input.GetKey(KeyCode.A))
        {
            moving = true;
            transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.S))
        {
            moving = true;
            transform.Translate(Vector3.down * moveSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.W))
        {
            moving = true;
            transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.D))
        {
            moving = true;
            transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
        }


        if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
        {
            moving = false;
        }

        // cloak if standing still
        cloakIfWalking();





        // remove old bullets and look at aiming direction
        currentMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        currentMousePos.z = 0; // flatten the vector into 2D

        currentPlayerPos = new Vector3(player.position.x, player.position.y, 0);
        // Debug.Log("playerPos2D: " + currentPlayerPos);
        //   Debug.Log("mousePos2D: " + currentMousePos);

        // bullet spawninig
        if (Input.GetKey(KeyCode.Mouse0) && Time.time > nextFire && moving)
        {
            nextFire = Time.time + fireRate;

            // mousePosition relative to scene NOT to camera (this would be Input.mousePosition ;) )

            Vector3 heading = currentMousePos - currentPlayerPos;
            float dist = heading.magnitude;
            Vector3 direction = heading / dist;
            GameObject bulletClone = Instantiate(bullet, currentPlayerPos + direction * bulletSpawnDistance, Quaternion.identity);
            currentPlayerPos.z = 0.22f;
            currentMousePos.z = 0.22f;

            //  bulletClone.transform.LookAt(currentMousePos);
            Rigidbody rb = bulletClone.GetComponent<Rigidbody>();
            rb.velocity = new Vector2(direction.x * bulletSpeed, direction.y * bulletSpeed);
            bulletList.AddFirst(bulletClone);
        }

        aimAtMouse();

        // sprite direction
        transform.LookAt(Camera.main.transform.position, -Vector3.up);
        removeDistantShots();
    }

    void cloakIfWalking()
    {
        // if player is moving
        if (false)
        {
            // show gunBarrel
            gun.GetComponent<SpriteRenderer>().enabled = true;
            //     Debug.Log("activated gunBarrel!");
        }
        else if (false)
        {
            // if he is standing still, hide gunBarrel
            gun.GetComponent<SpriteRenderer>().enabled = false;
            //     Debug.Log("deactivated gunBarrel!");
        }
    }

    void aimAtMouse()
    {

        currentPlayerPos.z = -0.1f;

        // Script for angle rotation towards mouse ( only works for squared Displays...)
        // TODO: make script for any display size
        Vector2 mousePos = Camera.main.ScreenToViewportPoint(Input.mousePosition);       // Mouse position on screen
        Vector2 gunPos = Camera.main.WorldToViewportPoint(gun.transform.position);        // gun position on screen
        Vector2 playerPos = Camera.main.WorldToViewportPoint(player.transform.position); // player position on screen
        Vector2 relobjpos = new Vector2(gunPos.x - 0.5f, gunPos.y - 0.5f);            //Set coordinates relative to object
              
        Vector2 relmousepos = new Vector2(mousePos.x - 0.5f, mousePos.y - 0.5f) - relobjpos;
        float angle = Vector2.Angle(-Vector2.up, relmousepos);    //Angle calculation

        if (relmousepos.x > 0)
        {
            angle = 360 - angle;
        }
        Quaternion quat = Quaternion.identity;
        quat.eulerAngles = new Vector3(0, 0, angle); //Changing angle

        // rotate gun
        gun.transform.rotation = quat;

        Vector2 mousePoss = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector3 heading = mousePoss - player.position;
        float distance = heading.magnitude;
        Vector3 direction = heading / distance;
        direction.y = -direction.y;
        direction.z = -0.5f;

        // manage distance between player and gun
        // not working nice.. looks like SHIT: gun.transform.position = Vector2.MoveTowards(player.transform.position, targetPos, gunSpawnDistance * Time.deltaTime);
      
        // this works better for the moment, but its still not perfect when you aim at your own player with the mouse
        gun.transform.position = player.transform.position + direction * gunSpawnDistance;

      


    }

	private void FixedUpdate()
	{
		
	}

	void removeDistantShots()
    {
        // debug
        foreach (GameObject bullet in bulletList)
        {
            Vector3 distance = currentPlayerPos - bullet.transform.position;
            //        Debug.Log("distance: " + distance.magnitude.ToString());
            if (distance.magnitude > shootingDistance)
            {
                Destroy(bullet);
                bulletList.Remove(bullet);
            }
        }
    }

    void generateIslands()
    {
        Debug.Log("called generateIslands!");
        MapGenerator generator = new MapGenerator(5);
        // starts itself when ready
    }

}