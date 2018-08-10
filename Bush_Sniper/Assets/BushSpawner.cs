using UnityEngine;

public class BushSpawner : MonoBehaviour {
    public GameObject nonPlayerBush = null;

	// Use this for initialization
	void Start () {
        int random = Random.Range(0,10);
        for (int i = 0; i < 20; i++)
        {
            Bounds bounds = GetComponent<SpriteRenderer>().bounds;
            float rangeX = bounds.size.x/2;
            float rangeY = bounds.size.y/2;
            float rangeZ = bounds.size.z;
            Debug.Log("Xrange of plane is: " + rangeX);
            Debug.Log("Yrange of plane is: " + rangeY);

            Instantiate(nonPlayerBush, new Vector3(Random.Range(-rangeX , rangeX ), Random.Range(-rangeY , rangeY ), 0), Quaternion.identity);

        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
