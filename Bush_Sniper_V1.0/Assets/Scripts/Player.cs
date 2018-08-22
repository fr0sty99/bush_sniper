using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Player : NetworkBehaviour
{

    [SyncVar] // this prefix means, when the value of this variable changes, it will be "pushed out" to all other clients
    private bool _isDead = false;
    public bool isDead
    {
        get { return _isDead; }
        protected set { _isDead = value; }
    }

    [SerializeField]
    private int maxHealth = 100;

    [SerializeField]
    private GameObject bulletTrailPrefab;

    [SerializeField]
    private GameObject muzzleFlashPrefab;

    [SerializeField]
    private GameObject islandPrefab;

    [SerializeField]
    private GameObject bridgePrefab;

    // TODO: refactor this shit
    private float muzzleRotationOffset = 180f; // needed to flip the image
    private float bridgeRotationOffset = 90f; // needed to flip the bridge from horizontal to verti

    [SerializeField]
    private float muzzleLifeTime = 0.02f;

    // mapGenerator variables
    public int mapSize = 3;
    public float islandSpawnOffset = 24f;
    public Island currentIsland;
    public Island startIsland;
    public Island[][] islands;
    public float bridgeSpawnOffset = 12f;

    // TODO: Until here!!


    // TODO: implement player skin


    [SyncVar] // this prefix means, when the value of this variable changes, it will be "pushed out" to all other clients
    private int currentHealt;

    [SerializeField]
    private Behaviour[] disableOnDeath;
    private bool[] wasEnabled;

    public void Setup()
    {
        wasEnabled = new bool[disableOnDeath.Length];
        for (int i = 0; i < wasEnabled.Length; i++)
        {
            wasEnabled[i] = disableOnDeath[i].enabled;
        }

        SetDefaults();
    }

    void Start()
    {
        if (isServer)
        {
            // generate Map with Depth-first search algorithm

            // For the setup we have A n*n  Array of Islands, which have no bridges ( see variables at top ) 

            // Algorithm-Notes:
            // 1. Start at a random island
            // 2. Mark the current island as visited, and get a list of its neighbors. For each neighbor, starting with a randomly selected neighbor:
            //      If this neighbor hasn't been visited, create a bridge between this island and that neighbor, and then recurse with that neighbor as the current island
            // finish :-) 


            // Initialize a 2D-Array of Islands with size n*n
            islands = new Island[mapSize][];
            for (int i = 0; i < islands.Length; i++)
            {
                islands[i] = new Island[mapSize];
            }

            for (int x = 0; x < islands.Length; x++)
            {
                for (int y = 0; y < islands.Length; y++)
                {
                    islands[x][y] = new Island(x, y);
                }
            }

            // 1. Start at a random island
            int random = Random.Range(0, islands.Length + islands[0].Length);
            int randomX = random / mapSize;
            int randomY = random % mapSize;

            currentIsland = islands[randomX][randomY];

            // recusive
            recursiveDFS(currentIsland);

            Debug.Log("Map geneartion finished.");

            // spawn map 

            for (int x = 0; x < islands.Length; x++)
            {
                for (int y = 0; y < islands.Length; y++)
                {
                    // create Island
                    Instantiate(islandPrefab, new Vector2(x * islandSpawnOffset, y * islandSpawnOffset), Quaternion.identity);

                    // create Bridges
                    if(islands[x][y].bridges[0]) 
                    {
                        // top
                        GameObject gameObject =  Instantiate(bridgePrefab, new Vector2(x * islandSpawnOffset, y * islandSpawnOffset + bridgeSpawnOffset), Quaternion.Euler(0,0, bridgeRotationOffset));
                        gameObject.name = "top of " + x + "|" + y;
                    }
                    if (islands[x][y].bridges[1])
                    {
                        // right
                        GameObject gameObject = Instantiate(bridgePrefab, new Vector2(x * islandSpawnOffset + bridgeSpawnOffset , y * islandSpawnOffset ), Quaternion.identity);
                        gameObject.name = "right of " + x + "|" + y;
                    }
                    if (islands[x][y].bridges[2])
                    {
                        // bottom
                        GameObject gameObject = Instantiate(bridgePrefab, new Vector2(x * islandSpawnOffset, y * islandSpawnOffset- bridgeSpawnOffset), Quaternion.Euler(0, 0, bridgeRotationOffset));
                        gameObject.name = "bottom of " + x + "|" + y;
                    }
                    if (islands[x][y].bridges[3])
                    {
                        // left
                        GameObject gameObject =  Instantiate(bridgePrefab, new Vector2(x * islandSpawnOffset - bridgeSpawnOffset, y * islandSpawnOffset), Quaternion.identity);
                        gameObject.name = "left of " + x + "|" + y;
                    }
                }
            }

            Debug.Log("Map spawning finished");
        }
    }

    void recursiveDFS(Island _island)
    {
        currentIsland = _island;

        if(currentIsland.visited) {
            return;
        } else {
        // mark the current island as visited
            currentIsland.visited = true;
        }

        // get a list of the current islands neighbors
        findIslandNeighbors(currentIsland);

        // foreach neighbor
        foreach(Island _neighbor in currentIsland.neighbors) {
            if (!_neighbor.visited)
            {
                createBridgeBetween(currentIsland, _neighbor);
            }

            recursiveDFS(_neighbor);
        }
    }

    public void createBridgeBetween(Island currentIsland, Island neighbor)
    {
        // We create the bridge only on one Island, because we would render it twice this way
        // neighbor on top of currentIsland
        if (neighbor.y + 1 == currentIsland.y && currentIsland.x == neighbor.x)
        {
            currentIsland.setTopBridge(true);
            neighbor.setBottomBridge(true);
        }

        // neighbor on the right of currentIsland
        if (neighbor.x - 1 == currentIsland.x && currentIsland.y == neighbor.y)
        {
            currentIsland.setRightBridge(true);
            neighbor.setLeftBridge(true);
        }

        // neighbor is on the bottom of currentIsland
        if (neighbor.y - 1 == currentIsland.y && currentIsland.x == neighbor.x)
        {
            currentIsland.setBottomBridge(true);
            neighbor.setTopBridge(true);
        }

        // neighbor is on the left of currentIsland
        if (neighbor.x + 1 == currentIsland.x && currentIsland.y == neighbor.y)
        {
            currentIsland.setLeftBridge(true);
            neighbor.setRightBridge(true);
        }
    }

    public void findIslandNeighbors(Island island)
    {
        // top
        if (island.y + 1 < mapSize-1)
        {
            island.neighbors.Add(islands[island.x][island.y + 1]);
        }
        // right
        if (island.x + 1 < mapSize-1)
        {
            island.neighbors.Add(islands[island.x + 1][island.y]);
        }
        // bottom
        if (island.y - 1 >= 0)
        {
            island.neighbors.Add(islands[island.x][island.y - 1]);
        }
        // left
        if (island.x - 1 >= 0)
        {
            island.neighbors.Add(islands[island.x - 1][island.y]);
        }
    }

    //void Update()
    //{
    //       if(!isLocalPlayer) {
    //           return;
    //       }

    //       if(Input.GetKeyDown(KeyCode.K)) {
    //           RpcTakeDamage(999);
    //       }
    //}

    [ClientRpc] // this method gets executed on every client if its called
    public void RpcSpawnShootEffect(Vector2 _pos, Quaternion _rot)
    {
        showBulletTrail(_pos, _rot);
        showMuzzle(_pos, _rot);
    }

    private void showMuzzle(Vector2 _pos, Quaternion _rot)
    {
        // TODO: Refactor this ugly piece of shit
        Vector3 muzzlePos = new Vector3(_pos.x, _pos.y, -1f);
        GameObject muzzleFlashObject = Instantiate(muzzleFlashPrefab, muzzlePos, _rot);
        muzzleFlashObject.transform.Rotate(0, 0, muzzleRotationOffset);
        //muzzleFlashObject.transform.parent = GetComponent<PlayerShoot>().weapon.firePoint;
        //float size = Random.Range(0.03f, 0.07f);
        //muzzleFlashObject.transform.localScale = new Vector2(size, size);
        Destroy(muzzleFlashObject, muzzleLifeTime);
    }

    private void showBulletTrail(Vector2 _pos, Quaternion _rot)
    {
        // spawn a bulletTrail and Destroy it after 1 second
        Destroy(Instantiate(bulletTrailPrefab, _pos, _rot), 1);
    }

    [ClientRpc] // this method gets executed on every client if its called
    public void RpcTakeDamage(int _amount)
    {
        if (isDead)
        {
            return;
        }
        currentHealt -= _amount;

        // only on server because this is the host, we deactivate all remotePlayer-Components
        Debug.Log(transform.name + " now has " + currentHealt + " Health.");

        if (currentHealt <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // TODO: die animation ///////////////////////////////
        isDead = true;

        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = false;
        }

        Collider2D _col = GetComponent<Collider2D>();
        if (_col != null)
        {
            _col.enabled = true;
        }

        Debug.Log(transform.name + " is DEAD!");

        StartCoroutine(Respawn());

    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(GameManager.instance.matchSettings.respawnTime);

        SetDefaults();
        Transform _spawnPoint = NetworkManager.singleton.GetStartPosition();
        transform.position = _spawnPoint.position;
        transform.rotation = _spawnPoint.rotation;

        Debug.Log(transform.name + " respawned.");
    }

    public void SetDefaults()
    {
        isDead = false;

        currentHealt = maxHealth;

        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = wasEnabled[i];
        }

        Collider2D _col = GetComponent<Collider2D>();
        if (_col != null)
        {
            _col.enabled = true;
        }
    }
}
