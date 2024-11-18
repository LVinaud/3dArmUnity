//This code defines the Node class, which is the basic component of the grid

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public bool walkable;
    public Vector3 worldPosition;
    public int gridX;
    public int gridY;
    public int gridZ;

    public int gCost;   //In A*, gCost is the distance between the starting point and the node
    public int hCost;   //In A*, hCost is the distance between the arrival point and the node

    public Node parent;

    public int layer; //later added for the 3d arm project, usedd to indicate if it is part of a layer 

    public Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY, int _gridZ, int layers)
    {
        this.walkable = _walkable;
        this.worldPosition = _worldPos;
        this.gridX = _gridX;
        this.gridY = _gridY;
        this.gridZ = _gridZ;
        this.layer = 0;//0 means a blank node, with no layer. not to be confuded with the unity layer, this is a ndoe not a gameobject!
        if (_walkable == false)
            this.layer = layers + 1;//layers + 1 will represent an obstacle
    }

    public int fCost
    {
        get{
            return gCost + hCost;
        }
    }
}