using UnityEngine;

public class BushSpawner : MonoBehaviour {

    int islandDiameter = 5;

	// Use this for initialization
	void Start () {
        int random = Random.Range(0,10);
        for (int i = 0; i < random; i++)
        {
            Instantiate(Resources.Load("Assets/NonPlayerBush") as GameObject, new Vector2(Random.Range(transform.position.x - islandDiameter , transform.position.x + islandDiameter ), 
                                                   Random.Range(transform.position.y - islandDiameter , transform.position.y + islandDiameter )), Quaternion.identity);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
