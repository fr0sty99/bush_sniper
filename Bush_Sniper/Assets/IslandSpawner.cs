using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandSpawner : MonoBehaviour {

	// Use this for initialization
	void Start () {
        var numIslands = Random.Range(0, 10);
        var numBridges = numIslands + (numIslands / 4);
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
