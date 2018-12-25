using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crate : MonoBehaviour {
    [SerializeField]
    int health = 20;

	// Use this for initialization
	void Start () {
		
	}

	// Update is called once per frame
	void Update () {
		
	}

    public void takeDamange(int damage) {
        health -= damage;
    }

    public int getHealth() {
        return health;
    }

    public void setHealth(int _health) {
        this.health = _health;
    }

    public void OnDestroy()
    {
        // TODO: run crate explosing animation
        // TODO: spawn weapon

    }

}
