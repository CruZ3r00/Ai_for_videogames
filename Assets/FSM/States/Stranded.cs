using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using System.IO;
using UnityEditor.Search;



public class Stranded
{
    Transform transform;
    AgentMovement movement;
    AgentController controller;
    Graph graph;
    Terrain terrain;
    private WaterSystem water;

    private GraphNode currentNode;
    private GraphNode randomTarget;
    private float safeHeight;
    TidalWalkerFSM fsm;
    public Stranded(Transform tran,AgentMovement m,AgentController c,Graph g,Terrain ter, WaterSystem w, TidalWalkerFSM f)
    {
        transform = tran;
        movement = m;
        controller = c;
        graph = g;
        terrain = ter;
        water = w;
        fsm = f;
    }
    public void Enter()
    {
        Debug.Log("sono in stranded");
        currentNode = movement.WorldToNode(transform.position);
        safeHeight = fsm.WaterIsRising() ? currentNode.height : 7.1f;
    }
    public void Stay()
    {
        safeHeight = fsm.WaterIsRising() ? currentNode.height : 7.1f;
        // Ogni frame → ricalcolo sempre il nodo attuale
        currentNode = movement.WorldToNode(transform.position);

        //nodi vicini safe
        List<GraphNode> safe = new List<GraphNode>();
        foreach (GraphNode n in PathfinderAStar.GetNeighbours(graph, currentNode))
        {
            if (n.height >= safeHeight && n != randomTarget)
                safe.Add(n);
        }
        if (safe.Count == 0)
        {
            return;
        }

        //scelgo un nodo safe random
        randomTarget = safe[Random.Range(0, safe.Count)];

        //path a quel nodo
        List<GraphNode> path = PathfinderAStar.FindFullPath(graph, currentNode, randomTarget);

        if (path != null && path.Count > 0)
        {
            movement.path = path;
            movement.currentIndex = 0;
        }
    }

}