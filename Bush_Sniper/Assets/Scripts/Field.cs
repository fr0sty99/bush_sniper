using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Field : MonoBehaviour
{
    public int x;
    public int y;
    public bool visited = false;
    public bool isCurrent = false;
    public Bridge[] bridges = new Bridge[4]; // top, right, bottom, left

    public Field(int x, int y) {
        this.x = x;
        this.y = y;
        bridges[0] = null;
        bridges[1] = null;
        bridges[2] = null;
        bridges[3] = null;
    }

    // Use this for initialization
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public bool hasNonVisitedNeighbors(Field[][] map)
    {
        if (getNonVisitedNeighbors(map).Count == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public ArrayList getNonVisitedNeighbors(Field[][] map)
    {
        ArrayList neighborList = new ArrayList();

        // top neighbor
        if (y > 0 && !map[getX()][getY() - 1].isVisited())
        {
            neighborList.Add(map[getX()][getY() - 1]);
        }

        // right neighbor
        if (x < map.Length - 1 && !map[getX() + 1][getY()].isVisited())
        {
            neighborList.Add(map[getX() + 1][getY()]);
        }

        // bottom
        if (y < map.Length - 1 && !map[getX()][getY() + 1].isVisited())
        {
            neighborList.Add(map[getX()][getY() + 1]);
        }

        // left
        if (x > 0 && !map[getX() - 1][getY()].isVisited())
        {
            neighborList.Add(map[getX() - 1][getY()]);
        }
        return neighborList;
    }

    public bool getCurrent()
    {
        return this.isCurrent;
    }

    public void setCurrent(bool value)
    {
        this.isCurrent = value;
    }

    public void setVisited(bool visited)
    {
        this.visited = visited;
    }

    public void setTopBridge(bool Bridge)
    {
        bridges[0].setValue(Bridge);
    }

    public void setRightBridge(bool Bridge)
    {
        bridges[1].setValue(Bridge);
    }

    public void setBottomBridge(bool Bridge)
    {
        bridges[2].setValue(Bridge);
    }

    public void setLeftBridge(bool Bridge)
    {
        bridges[3].setValue(Bridge);
    }

    public bool getTopBridge()
    {
        return bridges[0].getValue();
    }

    public bool getRightBridge()
    {
        return bridges[1].getValue();
    }

    public bool getBottomBridge()
    {
        return bridges[2].getValue();
    }

    public bool getLeftBridge()
    {
        return bridges[3].getValue();
    }

    public int getX()
    {
        return x;
    }

    public int getY()
    {
        return y;
    }

    public bool isVisited()
    {
        return visited;
    }
}