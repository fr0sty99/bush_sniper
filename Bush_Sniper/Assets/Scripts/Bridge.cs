using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bridge : MonoBehaviour
{
    public int x;
    public int y;
    public bool value;
    Direction direction;

    public Bridge(int x, int y, Direction direction) {
        this.x = x;
        this.y = y;
        this.direction = direction;
        this.value = true;
    }

    // Use this for initialization
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {

    }




     // getter and setter

    public void setValue(bool value)
    {
        this.value = value;
    }

    public bool getValue()
    {
        return value;
    }

    public void setDirection(Direction direction)
    {
        this.direction = direction;
    }

    public Direction getDirection()
    {
        return direction;
    }

    public int getX()
    {
        return x;
    }

    public int getY()
    {
        return y;
    }
}