using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerObject : MonoBehaviour
{

    public Rigidbody player = null;
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


    void Start()
    {
        player = GetComponent<Rigidbody>();

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






        // remove old bullets and look at aiming direction
        currentMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        currentMousePos.z = 0; // flatten the vector into 2D

        currentPlayerPos = new Vector3(player.position.x, player.position.y, 0);
        Debug.Log("playerPos2D: " + currentPlayerPos);

        // bullet spawninig
        if (Input.GetKey(KeyCode.Space) && Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;

            // mousePosition relative to scene NOT to camera (this would be Input.mousePosition ;) )

            Vector3 heading = currentMousePos - currentPlayerPos;
            float dist = heading.magnitude;
            Vector3 direction = heading / dist;
            GameObject bulletClone = Instantiate(bullet, currentPlayerPos /* + direction * 0.8f barrel length */, Quaternion.identity);
            currentPlayerPos.z = 0.22f;
            currentMousePos.z = 0.22f;
            gun.transform.position = currentPlayerPos;
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


    void aimAtMouse()
    {
        currentMousePos.z = 0.22f;
        currentPlayerPos.z = 0.22f;
        gun.transform.position = currentPlayerPos;
        gun.transform.LookAt(currentMousePos);
        gun.transform.position += gun.transform.forward * 0.2f;
    }

    void removeDistantShots()
    {
        // debug
        foreach (GameObject bullet in bulletList)
        {
            Vector3 distance = currentPlayerPos - bullet.transform.position;
            Debug.Log("distance: " + distance.magnitude.ToString());
            if (distance.magnitude > shootingDistance)
            {
                Destroy(bullet);
                bulletList.Remove(bullet);
            }
        }
    }

}