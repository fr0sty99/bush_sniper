﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Islandgenerator : MonoBehaviour {

    public GameObject island;

	// Use this for initialization
	void Start () {
        Instantiate(island, new Vector3(transform.position.x,transform.position.y, 0.5f), Quaternion.identity);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
