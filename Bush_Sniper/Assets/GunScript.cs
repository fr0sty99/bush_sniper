using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunScript : MonoBehaviour {
    public float distanceFromPlayer = 0.5f;
    public GameObject player;
    Vector2 playerPostition;
    Vector2 mousePosition;
    Vector2 distance;
    Vector2 direction;

	// Use this for initialization
	void Start () {
        
 
	}
	
	// Update is called once per frame
	void Update () {
        // get direction from player to mouse;


        // get mouse Position relative to the Scene
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // get playerPosition
        playerPostition = player.transform.position;
	}
}
