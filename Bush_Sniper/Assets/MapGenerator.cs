using UnityEngine;
using System.Collections;

public class MapGenerator
{
    // Island prefabs
    GameObject islandBridgetop;
    // Map
    int x, y = 0;
    int mapSize = 5; // f.e. 10 means 10x10 grid
    Field[][] map;
    ArrayList queue = new ArrayList();

    public MapGenerator(int size) {
        mapSize = size;
        this.Start();
    }

    // Use this for initialization
    void Start()
    {
        map = new Field[mapSize][];
        for (int i = 0; i < map.Length; i++)
        {
            map[i] = new Field[mapSize];
        }

        // init new map
        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                map[x][y] = new Field(x, y);
            }
        }

        startDFS();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void startDFS()
    {
        Debug.Log("Started DFS!");
        x = Random.Range(0, mapSize);
        y = Random.Range(0, mapSize);

        Field choosenNeighborField = null;
        Field currentField = map[x][y];
        bool mazeGenRunning = true;

        currentField.setVisited(true);
        currentField.setCurrent(true);

        while (mazeGenRunning)
        {
            Debug.Log("in DFS loop");
            if (currentField.hasNonVisitedNeighbors(map))
            {
                ArrayList neighborList = currentField.getNonVisitedNeighbors(map);
                choosenNeighborField = (Field)neighborList[Random.Range(0, neighborList.Count)];
            }
            else
            {
                while (choosenNeighborField == null || !choosenNeighborField.hasNonVisitedNeighbors(map))
                {
                    if (queue.Count > 0)
                    {
                        choosenNeighborField = (Field)queue[queue.Count];
                    }
                    else
                    {
                        mazeGenRunning = false;
                        Debug.Log("Map Generation finished... spawning Islands... ");
                        for (int i = 0; i < map.Length; i++) {
                            for (int j = 0; j < map.Length; j++) {
                                bool top = map[i][j].getTopBridge();
                                bool bottom = map[i][j].getTopBridge();
                                bool right = map[i][j].getTopBridge();
                                bool left = map[i][j].getTopBridge();
                                if(top && !left && !right && !bottom) {
                                    GameObject islandTop = (GameObject) MonoBehaviour.Instantiate(Resources.Load("islandBridgeTop"));
                                    islandTop.transform.position = new Vector2(0, 0);
                                    Debug.Log("Bridge Created: Top ");
                                }
                               
                            }
                        }
                        break;
                    }
                }
            }

            currentField.setCurrent(false);
            choosenNeighborField.setVisited(true);
            addBridgesBetweenCells(currentField, choosenNeighborField);
            currentField = choosenNeighborField;
            currentField.setCurrent(true);
            queue.Add(currentField);

        }
    }

    public void addBridgesBetweenCells(Field a, Field b)
    {
        // TODO: change names and behaving for this project 

        // b on top of a
        if (b.y + 1 == a.y && a.x == b.x)
        {
            a.setTopBridge(true);
            b.setBottomBridge(true);
        }

        // b on the right of a
        if (b.x - 1 == a.x && a.y == b.y)
        {
            a.setRightBridge(true);
            b.setLeftBridge(true);
        }

        // b is on the bottom of a
        if (b.y - 1 == a.y && a.x == b.x)
        {
            a.setBottomBridge(true);
            b.setTopBridge(true);
        }

        // b is on the left of a
        if (b.x + 1 == a.x && a.y == b.y)
        {
            a.setLeftBridge(true);
            b.setRightBridge(true);
        }
    }
}

