using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour {
    private Rigidbody2D rb;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        rb = GetComponent<Rigidbody2D>();
        rb.transform.position = rb.transform.position + Vector3.left;
	}
}
