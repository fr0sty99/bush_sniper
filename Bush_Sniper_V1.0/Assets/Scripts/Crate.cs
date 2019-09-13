using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crate : MonoBehaviour {
    [SerializeField]
    int health = 20;
    [SerializeField]
    List<GameObject> dropPrefabList;



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
        Debug.Log("Spawned drop");

        // gets destroyed after 30 seconds
        Destroy(Instantiate(getRandomDrop(), new Vector3(transform.position.x, transform.position.y, transform.position.z + 1f), Quaternion.identity), 30);

        // TODO: make dropped item rotate around its own axis
    }

    public GameObject getRandomDrop() {

        int i = Random.Range(0, dropPrefabList.Count);
        return dropPrefabList[i];
    }

}
