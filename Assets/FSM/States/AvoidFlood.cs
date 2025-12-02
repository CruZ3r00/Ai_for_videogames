using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;



public class AvoidFlood
{
    Transform transform;
    AgentMovement movement;
    AgentController controller;
    Graph graph;
    Terrain terrain;
    GraphNode safeTarget;
    GraphNode start;
    private WaterSystem water;
    public bool reached = false;
    public AvoidFlood(Transform tran,AgentMovement m,AgentController c,Graph g,Terrain ter, WaterSystem w)
    {
        transform = tran;
        movement = m;
        controller = c;
        graph = g;
        terrain = ter;
        water = w;
    }
    public void Enter()
    {
        Debug.Log("sono in avoid flood");
        reached = false;

        start = movement.WorldToNode(transform.position);
        safeTarget = SpawnerAStar.FindSpawnPoint(graph, start, 7.5f);

        if (safeTarget == null)
        {
            Debug.LogWarning("[AvoidFlood] Nessun nodo sicuro trovabile!");
            reached = true;
            return;
        }

        List<GraphNode> safePath = FloodAwareAStar.FindFullPath(graph, start, safeTarget, water, movement);

        if (safePath == null)
        {
            Debug.LogWarning("[AvoidFlood] Nessun path sicuro verso l’altura!");
            reached = true;
            return;
        }

        movement.path = safePath;
        movement.currentIndex = 0;
    }
    public void Stay()
    {
        if( transform.position.y >= 7.1f ) reached = true;
    }

    public void Exit()
    {
    }
 
}