using System.Collections;
using System;

[System.Serializable]
public class Island {

    public bool[] bridges = new bool[4]; // top, right, bottom, left
    public bool visited = false;
    public int x;
    public int y;
    public ArrayList neighbors = new ArrayList();
    public MatrixLayer layer;

    public Island(int _x, int _y) {
        x = _x;
        y = _y;

        bridges[0] = false;
        bridges[1] = false;
        bridges[2] = false;
        bridges[3] = false;
    }

<<<<<<< HEAD
    public ArrayList getUnvisitedNeighbors() {
        ArrayList list = new ArrayList();
        if(neighbors.Count > 0) {
            foreach(Island island in neighbors) {
                if (!island.visited) {
                    list.Add(island);
                }
            }
        }
        if(list.Count > 0) {
            return list;
        } else {
            return null;
        }
    }

=======
>>>>>>> master
    public void setTopBridge(bool  value)
    {
        bridges[0] = value;
    }

    public void setRightBridge(bool  value)
    {
        bridges[1] = value;
    }

    public void setBottomBridge(bool  value)
    {
        bridges[2] = value;
    }

    public void setLeftBridge(bool  value)
    {
        bridges[3] = value;
    }

    public  bool getTopBridge()
    {
        return bridges[0];
    }

    public  bool getRightBridge()
    {
        return bridges[1];
    }

    public  bool getBottomBridge()
    {
        return bridges[2];
    }

    public  bool getLeftBridge()
    {
        return bridges[3];
    }

}
