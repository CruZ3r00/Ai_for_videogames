using System;
using UnityEngine;
public class GraphNode
{
    public int x, z;
    public float height;
    public bool walkable;

    public float gCost = Mathf.Infinity;
    public float hCost;
    public float fCost => gCost + hCost;

    public GraphNode parent;

    public GraphNode(int x, int z, float height, bool walkable)
    {
        this.x = x;
        this.z = z;
        this.height = height;
        this.walkable = walkable;

        gCost = Mathf.Infinity;  
        hCost = 0f;
    }
}