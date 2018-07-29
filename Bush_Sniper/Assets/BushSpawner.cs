using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BushSpawner : MonoBehaviour {
    public GameObject nonPlayerBush = null;

	// Use this for initialization
	void Start () {
        int random = Random.Range(0,10);
        for (int i = 0; i < random; i++)
        {
            Bounds bounds = GetComponent<MeshRenderer>().bounds;
            float rangeX = bounds.extents.x;
            float rangeY = bounds.extents.y;
            float rangeZ = bounds.extents.z;
            Instantiate(nonPlayerBush, new Vector3(Random.Range(-rangeX, rangeX), Random.Range(-rangeY, rangeY), 0), Quaternion.identity);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
