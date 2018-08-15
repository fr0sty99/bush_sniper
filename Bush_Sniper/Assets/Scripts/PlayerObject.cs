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
            // bullet without travelSpeed, we use so called raycasts here to determine if we hit something
            /*
            // Does the ray intersect any objects excluding the player layer
            RaycastHit2D hit = Physics2D.Raycast(player.position, Camera.main.ScreenToWorldPoint(Input.mousePosition), shootingDistance, 0);
            if (hit != null)
            {
                Debug.DrawRay(transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition) * hit.distance, Color.yellow);
                Debug.Log("Did Hit: " + hit.collider);
            }
            else
            {
                Debug.DrawRay(transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition) * shootingDistance    , Color.white);
                Debug.Log("Did not Hit");
            }
            */
            // bullet with travelspeed 
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
        if (moving)
        {
            // show gunBarrel
            gun.GetComponent<SpriteRenderer>().enabled = true;
        }
        else
        {
            // if he is standing still, hide gunBarrel
            gun.GetComponent<SpriteRenderer>().enabled = false;
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









    /*



TODO:



The cube lives in 3D world space. Your cursor lives in 2D screen space. So "get near" means you first have to calculate the distance between the camera and a plane passing through the object you are testing parallel to the plane of the camera. This distance allows you to project the mouse position into the 3D world. If you are looking down the 'Z' axis, then that distance will be the difference in 'Z' values of the two objects. Once you calculate the distance, you can use Camera.ScreenToWorldPoint() to calculate the position of the mouse. Assuming looking down the 'Z' axis:

     var dist = Mathf.Abs(transform.position.z - Camera.manin.transform.position.z);
     var v3Pos = Vector3(Input.mousePosition.x, Input.mousePosition.y, dist);
     v3Pos = Camera.main.ScreenToWorldPoint(v3Pos);

Now you can calculate the distance between the 'cursor' and your object:

     var distanceBetween = Vector3.Distance(v3Pos, transform.position);

If your camera is moving around (i.e. not just looking down the 'Z' axis) for what you are doing here you can fudge the distance calculation and just use the distance between the camera and the object for distance:

     var dist = Vector3.Distance(Camera.main.transform.position, transform.position);

Or if for some reason you need a more accurate distance calculation, you can approach your problem using Unity's mathematical Plane class:

     var plane = new Plane(Camera.main.transform.forward, transform.position);
     var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
     var dist : float;
     if (plane.Raycast(ray, dist)) {
         var v3Pos = ray.GetPoint(dist);
         var distanceBetween = Vector3.Distance(v3Pos, transform.position);
     }





*/




    private void OnTriggerEnter2D(Collider2D collision)
	{
        Debug.Log("Collider: " + collision);

	}

	void generateIslands()
    {
        Debug.Log("called generateIslands!");
        MapGenerator generator = new MapGenerator(5);
        // starts itself when ready
    }

}