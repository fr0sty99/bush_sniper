using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
using System.Linq;
using UnityScript.Scripting.Pipeline;

public class MapSetup : MonoBehaviour
{
    // Serialized fields first
    [SerializeField]
    public const int mapSize = 4;
    [SerializeField]
    private const float islandSpawnSpacing = 24;
    [SerializeField]
    private const float bridgeSpawnSpacing = 8;
    [SerializeField]
    private const float bridgeColliderSpacing = 3;
    [SerializeField]
    private const float bigBridgeColliderSpacing = 4;
    [SerializeField]
    private GameObject bushPrefab;
    [SerializeField]
    private GameObject bridgePrefab;
    [SerializeField]
    private GameObject islandPrefab;
    [SerializeField]
    private GameObject smallCollider;
    [SerializeField]
    private GameObject bigCollider;
    [SerializeField]
    private float islandPositionZ = 1f; // height pos
    [SerializeField]
    private float bridgePositionZ = 0.9f; // height pos
    [SerializeField]
    private int minTrees;
    [SerializeField]
    private int maxTrees;

    private float angle90f = 90f; // used for flipping the bridge from horizontal to vertical
    private Island startIsland;
    private Island currentIsland;

    // TODO: we need to load on every client which is joining, hence we should move it to the nework part
    private Island[][] islands;
    private ArrayList stack;
    private List<GameObject> map = new List<GameObject>();

    // needed for island layering
    private MatrixLayer currentLayer;

    IEnumerator WaitOneSec()
    {
        yield return new WaitForSeconds(1);
    }

	public void Start()
    {
        Debug.Log("MapSetup started");

        generateMap();
        spawnMap();
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


        // recursive backtracker (DFS)
        stack = new ArrayList();
        int randomX = Random.Range(0, mapSize);
        int randomY = Random.Range(0, mapSize);
        currentIsland = islands[randomX][randomY];

        // recusive
        recursiveDFS(currentIsland);

        Debug.Log("Map geneartion finished.");
    }

    public void ClearMap() {
        foreach(GameObject obj in map) {
            Destroy(obj);
        }
    }

    public void spawnMap() 
    {
        for (int x = 0; x < islands.Length; x++)
        {
            for (int y = 0; y < islands.Length; y++)
            {
                // create Island
                GameObject islandInstance = Instantiate(islandPrefab, new Vector3(x * islandSpawnSpacing, y * islandSpawnSpacing, islandPositionZ), Quaternion.identity);
                islandInstance.name = "Island (" + x + "," + y + ") is on Layer: " + islands[x][y].layer;
                Debug.Log("Island (" + x + "," + y + ") is on Layer: " + islands[x][y].layer);

                // spawn some trees
                int randNumber = Random.Range(minTrees, maxTrees);
                ArrayList spawnedPositions = new ArrayList();

                for (int i = 0; i < randNumber+1; i++) {
                    
                    float randomPosX = Random.Range(-7f, 7f);
                    float randomPosY = Random.Range(-7f, 7f);
                    // check if new pos is not in range of already spawned bush
                    foreach (float[] pos in spawnedPositions)
                    {
                    }


                    // TODO: if the position is o.k , spawn tree and save pos.
                    float[] position = new float[2];
                    position[0] = randomPosX;
                    position[1] = randomPosY;
                    spawnedPositions.Add(position);

                    Instantiate(bushPrefab, new Vector3(randomPosX + x * islandSpawnSpacing, randomPosY + y * islandSpawnSpacing, islandPositionZ - 2f), Quaternion.identity);

                            // TODO: if the position is not o.k. get new randomPosX and Y
                }

                // create Bridges
                if (islands[x][y].bridges[0]) // top bridge (vertical)
                {
                    map.Add(Instantiate(bridgePrefab, new Vector3(x * islandSpawnSpacing, y * islandSpawnSpacing + bridgeSpawnSpacing, bridgePositionZ), Quaternion.Euler(0, 0, angle90f)));

                    map.Add(Instantiate(smallCollider, new Vector3(x * islandSpawnSpacing - bridgeColliderSpacing - bridgeColliderSpacing, y * islandSpawnSpacing + bridgeSpawnSpacing + bridgeColliderSpacing), Quaternion.identity));
                    map.Add(Instantiate(smallCollider, new Vector3(x * islandSpawnSpacing + bridgeColliderSpacing + bridgeColliderSpacing, y * islandSpawnSpacing + bridgeSpawnSpacing + bridgeColliderSpacing), Quaternion.identity));
                }
                else
                {
                    map.Add(Instantiate(bigCollider, new Vector3(x * islandSpawnSpacing, y * islandSpawnSpacing + bridgeSpawnSpacing + bigBridgeColliderSpacing), Quaternion.Euler(0, 0, angle90f)));
                
                }
                if (islands[x][y].bridges[1])// right bridge (horizontal)
                {
                    Instantiate(bridgePrefab, new Vector3(x * islandSpawnSpacing + bridgeSpawnSpacing, y * islandSpawnSpacing, bridgePositionZ), Quaternion.identity);

                        map.Add(Instantiate(smallCollider, new Vector3(x * islandSpawnSpacing + bridgeSpawnSpacing + (bridgeSpawnSpacing / 2), y * islandSpawnSpacing + bridgeSpawnSpacing - bridgeColliderSpacing), Quaternion.identity));
                        map.Add(Instantiate(smallCollider, new Vector3(x * islandSpawnSpacing + bridgeSpawnSpacing + (bridgeSpawnSpacing / 2), y * islandSpawnSpacing - (bridgeSpawnSpacing - bridgeColliderSpacing)), Quaternion.identity));
                }
                else // no right bridge
                {
                        map.Add(Instantiate(smallCollider, new Vector3(x * islandSpawnSpacing + bridgeSpawnSpacing + bridgeColliderSpacing, y * islandSpawnSpacing + (bridgeSpawnSpacing - bridgeColliderSpacing)), Quaternion.Euler(0, 0, angle90f)));
                    map.Add(Instantiate(smallCollider, new Vector3(x * islandSpawnSpacing + bridgeSpawnSpacing + bridgeColliderSpacing, y * islandSpawnSpacing), Quaternion.Euler(0, 0, angle90f)));
                            map.Add(Instantiate(smallCollider, new Vector3(x * islandSpawnSpacing + bridgeSpawnSpacing + bridgeColliderSpacing, y * islandSpawnSpacing - (bridgeSpawnSpacing - bridgeColliderSpacing)), Quaternion.Euler(0, 0, angle90f)));
                }
                if (islands[x][y].bridges[2])
                {
                    // bottom
                    map.Add(Instantiate(bridgePrefab, new Vector3(x * islandSpawnSpacing, y * islandSpawnSpacing - bridgeSpawnSpacing, bridgePositionZ), Quaternion.Euler(0, 0, angle90f)));

                        map.Add(Instantiate(smallCollider, new Vector3(x * islandSpawnSpacing - bridgeColliderSpacing - bridgeColliderSpacing, y * islandSpawnSpacing - bridgeSpawnSpacing - bridgeColliderSpacing), Quaternion.identity));
                        map.Add(Instantiate(smallCollider, new Vector3(x * islandSpawnSpacing + bridgeColliderSpacing + bridgeColliderSpacing, y * islandSpawnSpacing - bridgeSpawnSpacing - bridgeColliderSpacing), Quaternion.identity));
                }
                else
                {
                    map.Add(Instantiate(smallCollider, new Vector3(x * islandSpawnSpacing, y * islandSpawnSpacing - bridgeSpawnSpacing - bridgeColliderSpacing, bridgePositionZ), Quaternion.identity));
                            map.Add(Instantiate(smallCollider, new Vector3(x * islandSpawnSpacing - (bridgeSpawnSpacing - bridgeColliderSpacing), y * islandSpawnSpacing - bridgeSpawnSpacing - bridgeColliderSpacing), Quaternion.identity));
                            map.Add(Instantiate(smallCollider, new Vector3(x * islandSpawnSpacing + (bridgeSpawnSpacing - bridgeColliderSpacing), y * islandSpawnSpacing - bridgeSpawnSpacing - bridgeColliderSpacing), Quaternion.identity));
                }
                if (islands[x][y].bridges[3])
                {
                    // left
                    map.Add(Instantiate(bridgePrefab, new Vector3(x * islandSpawnSpacing - bridgeSpawnSpacing - bridgeColliderSpacing, y * islandSpawnSpacing, bridgePositionZ), Quaternion.identity));

                            map.Add(Instantiate(smallCollider, new Vector3(x * islandSpawnSpacing - bridgeSpawnSpacing - (bridgeSpawnSpacing / 2), y * islandSpawnSpacing + (bridgeSpawnSpacing - bridgeColliderSpacing)), Quaternion.identity));
                            map.Add(Instantiate(smallCollider, new Vector3(x * islandSpawnSpacing - bridgeSpawnSpacing - (bridgeSpawnSpacing / 2), y * islandSpawnSpacing - (bridgeSpawnSpacing - bridgeColliderSpacing)), Quaternion.identity));
                }
                else
                {
                            map.Add(Instantiate(smallCollider, new Vector3(x * islandSpawnSpacing - bridgeSpawnSpacing - bridgeColliderSpacing, y * islandSpawnSpacing + (bridgeSpawnSpacing - bridgeColliderSpacing)), Quaternion.Euler(0, 0, angle90f)));
                            map.Add(Instantiate(smallCollider, new Vector3(x * islandSpawnSpacing - bridgeSpawnSpacing - bridgeColliderSpacing, y * islandSpawnSpacing), Quaternion.Euler(0, 0, angle90f)));
                            map.Add(Instantiate(smallCollider, new Vector3(x * islandSpawnSpacing - bridgeSpawnSpacing - bridgeColliderSpacing, y * islandSpawnSpacing - (bridgeSpawnSpacing - bridgeColliderSpacing)), Quaternion.Euler(0, 0, angle90f)));

                }
            }
        }

        Debug.Log("Map spawning finished");
    }

    bool unvisitedCellExists() {
        foreach (Island[] _islandContainer in islands) {
            foreach(Island _island in _islandContainer) {
                if(!_island.visited) {
                    return true;
                }
            }
        }
        return false;
    }

    void recursiveDFS(Island _island)
    {
        //    Make the initial cell the current cell and mark it as visited
        currentIsland = _island;
        currentIsland.visited = true;

        //While there are unvisited cells
        while (unvisitedCellExists())
        {
            //If the current cell has any neighbours which have not been visited
            if (findUnvisitedIslandNeighbors(currentIsland))
            {
                //    Choose randomly one of the unvisited neighbours
                ArrayList possibleNeighbors = currentIsland.getUnvisitedNeighbors();
                if (possibleNeighbors != null) {
                    object[] shuffleArray = possibleNeighbors.ToArray();
                    int rand = Random.Range(0, currentIsland.neighbors.Count);
                    Island _randomlyChoosenNeighbor = (Island)shuffleArray[rand];
                    Debug.Log("neighbor found: x" + _randomlyChoosenNeighbor.x + ",y" + _randomlyChoosenNeighbor.y);
                    currentIsland.neighbors = new ArrayList(); // reset neighbors
                    //    Push the current cell to the stack
                    stack.Add(currentIsland);

                    //    Remove the wall between the current cell and the chosen cell
                    createBridgeBetween(currentIsland, _randomlyChoosenNeighbor);

                    //    Make the chosen cell the current cell and mark it as visited
                    currentIsland = _randomlyChoosenNeighbor;
                    currentIsland.visited = true;
                }
            }                
            //Else if stack is not empty
            else if (stack.Count > 0)
            {
                Debug.Log("stack size before pop: " + stack.Count);
                //Pop a cell from the stack and make it the current cell
                currentIsland = (Island) stack[0];
                stack.RemoveAt(0);
                Debug.Log("stack size after pop: " + stack.Count);
            }
        }
    }

    void createBridgeBetween(Island _currentIsland, Island neighbor)
    {
        // We create the bridge only on one Island, because we would render it twice this way
        // neighbor on top of currentIsland
        if (neighbor.y - 1 == _currentIsland.y && _currentIsland.x == neighbor.x)
        {
            _currentIsland.setTopBridge(true);
            neighbor.setBottomBridge(true);
        }

        // neighbor on the right of currentIsland
        if (neighbor.x - 1 == _currentIsland.x && _currentIsland.y == neighbor.y)
        {
            _currentIsland.setRightBridge(true);
            neighbor.setLeftBridge(true);
        }

        // neighbor is on the bottom of currentIsland
        if (neighbor.y + 1 == _currentIsland.y && _currentIsland.x == neighbor.x)
        {
            _currentIsland.setBottomBridge(true);
            neighbor.setTopBridge(true);
        }

        // neighbor is on the left of currentIsland
        if (neighbor.x + 1 == _currentIsland.x && _currentIsland.y == neighbor.y)
        {
            _currentIsland.setLeftBridge(true);
            neighbor.setRightBridge(true);
        }
    }

    bool findUnvisitedIslandNeighbors(Island island)
    {
        // top
        if (island.y + 1 < mapSize)
        {
            if (!islands[island.x][island.y + 1].visited && !island.neighbors.Contains(islands[island.x][island.y + 1]))
            {
                island.neighbors.Add(islands[island.x][island.y + 1]);
            }
        }
        // right
        if (island.x + 1 < mapSize)
        {
            if (!islands[island.x + 1][island.y].visited && !island.neighbors.Contains(islands[island.x + 1][island.y]))
            {
                island.neighbors.Add(islands[island.x + 1][island.y]);
            }
        }
        // bottom
        if (island.y - 1 >= 0)
        {
            if (!islands[island.x][island.y - 1].visited && !island.neighbors.Contains(islands[island.x][island.y - 1]))
            {
                island.neighbors.Add(islands[island.x][island.y - 1]);
            }
        }
        // left
        if (island.x - 1 >= 0)
        {
            if (!islands[island.x - 1][island.y].visited && !island.neighbors.Contains(islands[island.x - 1][island.y]))
            {
                island.neighbors.Add(islands[island.x - 1][island.y]);
            }
        }

        Debug.Log("Island x,y: " + island.x + "," + island.y + " | has neighbors: ");
        for (int i = 0; i < island.neighbors.Count; i++)
        {
            Debug.Log("Neighbor #" + i + ": " + ((Island)island.neighbors[i]).x + " , " + ((Island)island.neighbors[i]).y);
        }

        // neighbor found
        if(island.neighbors.Count > 0) {
            return true;
        }

        // no neigbor found
        return false;
    }
}
