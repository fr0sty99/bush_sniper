using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour {
    public Vector3 heading;
    public GameObject player;
    public Rigidbody rb;
    public Vector3 direction;
    public float movementSpeed = 0.1f;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        transform.position += transform.forward * movementSpeed;
    }

}
