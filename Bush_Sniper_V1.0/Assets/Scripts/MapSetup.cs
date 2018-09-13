using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class MapSetup : NetworkBehaviour
{
    // Serialized fields first
    [SerializeField]
    private const int mapSize = 4;
    [SerializeField]
    private const float islandSpawnSpacing = 24;
    [SerializeField]
    private const float bridgeSpawnSpacing = 8;
    [SerializeField]
    private const float bridgeColliderSpacing = 3;
    [SerializeField]
    private const float bigBridgeColliderSpacing = 4;
    [SerializeField]
    private GameObject bridgePrefab;
    [SerializeField]
    private GameObject islandPrefab;
    [SerializeField]
    private GameObject smallCollider;
    [SerializeField]
    private GameObject bigCollider;

    private float angle90f = 90f; // used for flipping the bridge from horizontal to vertical

    private Island currentIsland;
    private Island startIsland;

    // TODO: we need to load on every client which is joining, hence we should move it to the nework part
    private Island[][] islands;
    private List<GameObject> map = new List<GameObject>();

    // needed for island layering
    private MatrixLayer currentLayer;
    private int stepsDone = 0;

    [SyncVar]
    private bool mapGenerated = false;

	public override void OnStartServer()
	{
        generateMap();
	}

	public override void OnStartClient()
	{
        Debug.Log("MapSetup started");
        while(!mapGenerated) {
            StartCoroutine(WaitOneSec());
        }
        spawnMap();
	}

    IEnumerator WaitOneSec()
    {
        yield return new WaitForSeconds(1);
    }

	private void OnDisconnectedFromServer(NetworkDisconnection info)
	{
        CmdClearMap();
	}

	public void Start()
    {
        //Debug.Log("MapSetup started");
        //if (!mapGenerated)
        //{
        //    generateMap();
        //}
        //spawnMap();
    }

    public void generateMap()
    {

        #region notes about algorith
        // generate Map with Depth-first search algorithm

        // For the setup we have A n*n  Array of Islands, which have no bridges ( see variables at top ) 

        // Algorithm-Notes:
        // 1. Start at a random island
        // 2. Mark the current island as visited, and get a list of its neighbors. For each neighbor, starting with a randomly selected neighbor:
        //      If this neighbor hasn't been visited, create a bridge between this island and that neighbor, and then recurse with that neighbor as the current island
        // finish :-) 

        #endregion
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

        // we want them islands to sink overtime, so we mark them with layers
        // we start at the highest island layer (A). This is the island which is the last sinking. After every iteration of the recusive method we increment the Layer

        // 1. Start at a random island
        int randomX = Random.Range(0, mapSize);
        int randomY = Random.Range(0, mapSize);

        currentIsland = islands[randomX][randomY];
        currentIsland.layer = MatrixLayer.A;

        stepsDone++;

        // recusive
        recursiveDFS(currentIsland);

        mapGenerated = true;
        Debug.Log("Map geneartion finished.");
    }

    [Command]
    public void CmdClearMap() {
        foreach(GameObject obj in map) {
            Destroy(obj);
        }
    }

    public void spawnMap() // todo: maybe change small colliders with big colliders?
    {
        for (int x = 0; x < islands.Length; x++)
        {
            for (int y = 0; y < islands.Length; y++)
            {
                // create Island
                GameObject islandInstance = Instantiate(islandPrefab, new Vector2(x * islandSpawnSpacing, y * islandSpawnSpacing), Quaternion.identity);
                islandInstance.name = "Island (" + x + "," + y + ") is on Layer: " + islands[x][y].layer;
                Debug.Log("Island (" + x + "," + y + ") is on Layer: " + islands[x][y].layer);

                // create Bridges
                if (islands[x][y].bridges[0]) // top bridge (vertical)
                {
                    map.Add(Instantiate(bridgePrefab, new Vector2(x * islandSpawnSpacing, y * islandSpawnSpacing + bridgeSpawnSpacing), Quaternion.Euler(0, 0, angle90f)));

                    map.Add(Instantiate(smallCollider, new Vector2(x * islandSpawnSpacing - bridgeColliderSpacing - bridgeColliderSpacing, y * islandSpawnSpacing + bridgeSpawnSpacing + bridgeColliderSpacing), Quaternion.identity));
                    map.Add(Instantiate(smallCollider, new Vector2(x * islandSpawnSpacing + bridgeColliderSpacing + bridgeColliderSpacing, y * islandSpawnSpacing + bridgeSpawnSpacing + bridgeColliderSpacing), Quaternion.identity));
                }
                else
                {
                    map.Add(Instantiate(bigCollider, new Vector2(x * islandSpawnSpacing, y * islandSpawnSpacing + bridgeSpawnSpacing + bigBridgeColliderSpacing), Quaternion.Euler(0, 0, angle90f)));
                
                }
                if (islands[x][y].bridges[1])// right bridge (horizontal)
                {
                    Instantiate(bridgePrefab, new Vector2(x * islandSpawnSpacing + bridgeSpawnSpacing, y * islandSpawnSpacing), Quaternion.identity);

                        map.Add(Instantiate(smallCollider, new Vector2(x * islandSpawnSpacing + bridgeSpawnSpacing + (bridgeSpawnSpacing / 2), y * islandSpawnSpacing + bridgeSpawnSpacing - bridgeColliderSpacing), Quaternion.identity));
                        map.Add(Instantiate(smallCollider, new Vector2(x * islandSpawnSpacing + bridgeSpawnSpacing + (bridgeSpawnSpacing / 2), y * islandSpawnSpacing - (bridgeSpawnSpacing - bridgeColliderSpacing)), Quaternion.identity));
                }
                else // no right bridge
                {
                        map.Add(Instantiate(smallCollider, new Vector2(x * islandSpawnSpacing + bridgeSpawnSpacing + bridgeColliderSpacing, y * islandSpawnSpacing + (bridgeSpawnSpacing - bridgeColliderSpacing)), Quaternion.Euler(0, 0, angle90f)));
                    map.Add(Instantiate(smallCollider, new Vector2(x * islandSpawnSpacing + bridgeSpawnSpacing + bridgeColliderSpacing, y * islandSpawnSpacing), Quaternion.Euler(0, 0, angle90f)));
                            map.Add(Instantiate(smallCollider, new Vector2(x * islandSpawnSpacing + bridgeSpawnSpacing + bridgeColliderSpacing, y * islandSpawnSpacing - (bridgeSpawnSpacing - bridgeColliderSpacing)), Quaternion.Euler(0, 0, angle90f)));
                }
                if (islands[x][y].bridges[2])
                {
                    // bottom
                        map.Add(Instantiate(bridgePrefab, new Vector2(x * islandSpawnSpacing, y * islandSpawnSpacing - bridgeSpawnSpacing), Quaternion.Euler(0, 0, angle90f)));

                        map.Add(Instantiate(smallCollider, new Vector2(x * islandSpawnSpacing - bridgeColliderSpacing - bridgeColliderSpacing, y * islandSpawnSpacing - bridgeSpawnSpacing - bridgeColliderSpacing), Quaternion.identity));
                        map.Add(Instantiate(smallCollider, new Vector2(x * islandSpawnSpacing + bridgeColliderSpacing + bridgeColliderSpacing, y * islandSpawnSpacing - bridgeSpawnSpacing - bridgeColliderSpacing), Quaternion.identity));
                }
                else
                {
                        map.Add(Instantiate(smallCollider, new Vector2(x * islandSpawnSpacing, y * islandSpawnSpacing - bridgeSpawnSpacing - bridgeColliderSpacing), Quaternion.identity));
                            map.Add(Instantiate(smallCollider, new Vector2(x * islandSpawnSpacing - (bridgeSpawnSpacing - bridgeColliderSpacing), y * islandSpawnSpacing - bridgeSpawnSpacing - bridgeColliderSpacing), Quaternion.identity));
                            map.Add(Instantiate(smallCollider, new Vector2(x * islandSpawnSpacing + (bridgeSpawnSpacing - bridgeColliderSpacing), y * islandSpawnSpacing - bridgeSpawnSpacing - bridgeColliderSpacing), Quaternion.identity));
                }
                if (islands[x][y].bridges[3])
                {
                    // left
                    map.Add(Instantiate(bridgePrefab, new Vector2(x * islandSpawnSpacing - bridgeSpawnSpacing - bridgeColliderSpacing, y * islandSpawnSpacing), Quaternion.identity));

                            map.Add(Instantiate(smallCollider, new Vector2(x * islandSpawnSpacing - bridgeSpawnSpacing - (bridgeSpawnSpacing / 2), y * islandSpawnSpacing + (bridgeSpawnSpacing - bridgeColliderSpacing)), Quaternion.identity));
                            map.Add(Instantiate(smallCollider, new Vector2(x * islandSpawnSpacing - bridgeSpawnSpacing - (bridgeSpawnSpacing / 2), y * islandSpawnSpacing - (bridgeSpawnSpacing - bridgeColliderSpacing)), Quaternion.identity));
                }
                else
                {
                            map.Add(Instantiate(smallCollider, new Vector2(x * islandSpawnSpacing - bridgeSpawnSpacing - bridgeColliderSpacing, y * islandSpawnSpacing + (bridgeSpawnSpacing - bridgeColliderSpacing)), Quaternion.Euler(0, 0, angle90f)));
                            map.Add(Instantiate(smallCollider, new Vector2(x * islandSpawnSpacing - bridgeSpawnSpacing - bridgeColliderSpacing, y * islandSpawnSpacing), Quaternion.Euler(0, 0, angle90f)));
                            map.Add(Instantiate(smallCollider, new Vector2(x * islandSpawnSpacing - bridgeSpawnSpacing - bridgeColliderSpacing, y * islandSpawnSpacing - (bridgeSpawnSpacing - bridgeColliderSpacing)), Quaternion.Euler(0, 0, angle90f)));

                }
            }
        }

        Debug.Log("Map spawning finished");
    }

    void assignLayer(Island island)
    {
        switch (stepsDone)
        {
            case 0:
                island.layer = MatrixLayer.A;
                break;
            case 1:
                island.layer = MatrixLayer.B;
                break;
            case 2:
                island.layer = MatrixLayer.C;
                break;
            case 3:
                island.layer = MatrixLayer.D;
                break;
            case 4:
                island.layer = MatrixLayer.E;
                break;
            case 5:
                island.layer = MatrixLayer.F;
                break;
            case 6:
                island.layer = MatrixLayer.G;
                break;
            case 7:
                island.layer = MatrixLayer.H;
                break;
        }
    }

    // TODO: implement recursive backtracker instead of DFS
    void recursiveDFS(Island _island)
    {
        currentIsland = _island;

        if (currentIsland.visited)
        {
            return;
        }
        else
        {
            // mark the current island as visited
            currentIsland.visited = true;
        }

        // get a list of the current islands neighbors
        findUnvisitedIslandNeighbors(currentIsland);

        object[] shuffledArray = currentIsland.neighbors.ToArray();
        for (int i = 0; i < currentIsland.neighbors.Count; i++)
        {
            int rand = Random.Range(0, currentIsland.neighbors.Count);
            object temp = shuffledArray[rand];
            shuffledArray[rand] = shuffledArray[i];
            shuffledArray[i] = temp;
        }

        // assign layer before recursive call 
        foreach (Island _unvisitedNeighbor in shuffledArray)
        {
            assignLayer(_unvisitedNeighbor); // assign height layer
            stepsDone++;
        }


        // foreach neighbor
        foreach (Island _unvisitedNeighbor in shuffledArray)
        {
            assignLayer(_unvisitedNeighbor); // assign height layer
            createBridgeBetween(currentIsland, _unvisitedNeighbor);
            recursiveDFS(_unvisitedNeighbor);
        }

    }

    void createBridgeBetween(Island currentIsland, Island neighbor)
    {
        // We create the bridge only on one Island, because we would render it twice this way
        // neighbor on top of currentIsland
        if (neighbor.y - 1 == currentIsland.y && currentIsland.x == neighbor.x)
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
        if (neighbor.y + 1 == currentIsland.y && currentIsland.x == neighbor.x)
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

    void findUnvisitedIslandNeighbors(Island island)
    {
        // top
        if (island.y + 1 < mapSize)
        {
            if (!islands[island.x][island.y + 1].visited)
            {

            }
            island.neighbors.Add(islands[island.x][island.y + 1]);
        }
        // right
        if (island.x + 1 < mapSize)
        {
            if (!islands[island.x + 1][island.y].visited)
            {

            }
            island.neighbors.Add(islands[island.x + 1][island.y]);
        }
        // bottom
        if (island.y - 1 >= 0)
        {
            if (!islands[island.x][island.y - 1].visited)
            {

            }
            island.neighbors.Add(islands[island.x][island.y - 1]);
        }
        // left
        if (island.x - 1 >= 0)
        {
            if (!islands[island.x - 1][island.y].visited)
            {

            }
            island.neighbors.Add(islands[island.x - 1][island.y]);
        }

        Debug.Log("Island x,y: " + island.x + "," + island.y + " | has neighbors: ");
        for (int i = 0; i < island.neighbors.Count; i++)
        {
            Debug.Log("Neighbor #" + i + ": " + ((Island)island.neighbors[i]).x + " , " + ((Island)island.neighbors[i]).y);
        }
    }



}
