using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rifle : PlayerWeapon {

	// Use this for initialization
	void Start () {
        name = "Rifle";
        fireRate = 1;
        damage = 5;
        range = 30f;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
