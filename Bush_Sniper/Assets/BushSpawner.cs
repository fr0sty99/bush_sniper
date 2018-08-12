using UnityEngine;

public class BushSpawner : MonoBehaviour
{

    int islandDiameter = 5;
    int bushMaxCount = 5;

    // Use this for initialization
    void Start()
    {
        int random = Random.Range(0, bushMaxCount);
        for (int i = 0; i < random; i++)
        {
            int rand1 = (int)Random.Range(transform.position.x - islandDiameter, transform.position.x + islandDiameter);
            int rand2 = (int)Random.Range(transform.position.y - islandDiameter, transform.position.y + islandDiameter);

            Debug.Log("Spawning bush:"+transform.position);
            Instantiate(Resources.Load<GameObject>("NonPlayerBush"), new Vector2(rand1,
                                                   rand2), Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
