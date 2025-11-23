using System;
using UnityEngine;
/*
    rappresenta ogni singolo punto della griglia per il pathfinding
    usata per conversione tra mondo e griglia in agent movement
    lettura e scrittura dei costi nei vari finder
*/
public class GraphNode
{
    public int x, z;//coordinate del nodo nella griglia
    public float height;//ualtezza in unita world
    public bool walkable;//true se l'agente puo camminarci

    //costo iniziale dal nodo di partenza ad arrivare a questo nodo
    public float gCost = Mathf.Infinity; 
    //stima euristica del costo finale 
    public float hCost;
    public float fCost => gCost + hCost;

    //nodo precedente per ricostruire il percorso
    public GraphNode parent;

    //costruttore per impostare i campi base
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