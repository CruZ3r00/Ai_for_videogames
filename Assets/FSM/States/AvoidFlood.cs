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
        movement.canMove = false;
        reached = false;
        start = movement.WorldToNode(movement.transform.position);

        safeTarget = SpawnerAStar.FindSpawnPoint(graph, start, 7.5f);
        if (safeTarget == null)
        {
            Debug.LogWarning("[AvoidFlood] Nessun nodo sicuro trovabile!");
            return;
        }
        
        List<GraphNode> safePath = PathfinderAStar.FindFullPath(graph, start, safeTarget);
        movement.path = safePath;
        movement.currentIndex = 0;
    }
    public void Stay()
    {
        movement.canMove = true;
        if( transform.position.y >= 7.1f ) reached = true;
    }

    public void Exit()
    {
        movement.canMove = false;
    }
 
}