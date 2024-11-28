//This code is responsible for creating the three-dimensional grid and for rendering it

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gridi : MonoBehaviour
{
    public LayerMask unwalkableMask;    //layer designated for obstacles
    public float nodeRadius;
    Node[,,] grid;
    public int layers = 5; //how many layers of distance to consider when looking at obstacles, it means that if the layers is set to 3, there will be a 3 node distance from obstacles until it is not anymore calculated

    public List<Node> path;
    private Vector3 gridWorldSize;
    public float nodeDiameter;
    private int gridSizeX, gridSizeY, gridSizeZ;

    private void Awake()    //takes the values ​​set in unity and passes the values ​​to private variables

    {
        gridWorldSize = GetComponent<CreateScene>().worldSize;
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        gridSizeZ = Mathf.RoundToInt(gridWorldSize.z / nodeDiameter);
    }

    public void createGrid()   //creation of the array that stores each cube of the collision

    {
        grid = new Node[gridSizeX, gridSizeY, gridSizeZ];
        
        // worldBottomLeft is in one of the corners of the grid
        // and will be the reference to get the position of objects within the grid
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.z / 2 - Vector3.up * gridWorldSize.y / 2;
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                for (int z = 0; z < gridSizeZ; z++) 
                {
                    Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (z * nodeDiameter + nodeRadius) + Vector3.up * (y * nodeDiameter + nodeRadius);
                    bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));
                    grid[x, y, z] = new Node(walkable, worldPoint, x, y, z, layers);
                }
            }
        }

        // with the grid created, I will now add the layers 
        for(int i = 1; i < layers; i++) {
            for (int x = 0; x < gridSizeX; x++)
            {
                for (int y = 0; y < gridSizeY; y++)
                {
                    for (int z = 0; z < gridSizeZ; z++) 
                    {
                        if((grid[x, y, z]).layer == i - 1) {//it means it is an obstacle or the previous layer and the neighbours need to be considered
                            List <Node> nrs = GetNeighbours(grid[x, y, z], 1);
                            foreach(Node element in nrs) {
                                element.layer = Mathf.Min(i, element.layer);
                            }
                        }
                    }
                }
            }
        }
    }

    public void recreateGrid(Vector3 oldPos, Vector3 newPos) {

        Node oldNode = NodeFromWorldPoint(oldPos);
        oldNode.layer = layers;
        List<Node> oldNeighbours = GetNeighbours(oldNode, layers);
        foreach(Node element in oldNeighbours){
            element.layer = layers;
        }
        Node newNode = NodeFromWorldPoint(newPos);
        newNode.layer = 0;
        for(int x = -layers; x<=layers; x++)
        {
            for (int y = -layers; y <= layers; y++)
            {
                for (int z = -layers; z <= layers; z++)
                {
                    if (x == 0 && y == 0 && z == 0) continue;

                    int checkX = newNode.gridX + x;
                    int checkY = newNode.gridY + y;
                    int checkZ = newNode.gridZ + z;

                    if(checkX >= 0 && checkX < gridSizeX && checkY>=0 && checkY< gridSizeY && checkZ >= 0 && checkZ < gridSizeZ)
                    {
                        grid[checkX, checkY, checkZ].layer = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y), Mathf.Abs(z));
                    }
                }
            }
        }

    }

    public List<Node> GetNeighbours(Node node, int depth)
    {
        List<Node> neighbours = new List<Node>();

        for(int x = -depth; x<=depth; x++)
        {
            for (int y = -depth; y <= depth; y++)
            {
                for (int z = -depth; z <= depth; z++)
                {
                    if (x == 0 && y == 0 && z == 0) continue;

                    int checkX = node.gridX + x;
                    int checkY = node.gridY + y;
                    int checkZ = node.gridZ + z;

                    if(checkX >= 0 && checkX < gridSizeX && checkY>=0 && checkY< gridSizeY && checkZ >= 0 && checkZ < gridSizeZ)
                    {
                        neighbours.Add(grid[checkX, checkY, checkZ]);
                    }
                }
            }
        }

        return neighbours;
    }

    public Node NodeFromWorldPoint(Vector3 worldPosition)   //this method receives a position and locates the node in the grid
    {
        float percentX = (worldPosition.x - transform.position.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.y - transform.position.y + gridWorldSize.y / 2) / gridWorldSize.y;
        float percentZ = (worldPosition.z - transform.position.z + gridWorldSize.z / 2) / gridWorldSize.z;

        percentX = Mathf.Clamp01(percentX); //Clamp 01 limits the value between 0 and 1, preventing code breakage.
        percentY = Mathf.Clamp01(percentY);
        percentZ = Mathf.Clamp01(percentZ);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        int z = Mathf.RoundToInt((gridSizeZ - 1) * percentZ);
        
        return grid[x, y, z];

    }

}