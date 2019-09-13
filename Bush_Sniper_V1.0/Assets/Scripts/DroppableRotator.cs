using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DroppableRotator : MonoBehaviour {
    float speed = 75f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(Vector3.forward, speed * Time.deltaTime);
    }

	// TODO PICKUP ITEM
}