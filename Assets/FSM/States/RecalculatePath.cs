using UnityEngine;
using System.Collections;
using Unity.VisualScripting.FullSerializer;
using System.Collections.Generic;



public class RecalculatePath
{
    Transform transform;
    AgentMovement movement;
    AgentController controller;
    Graph graph;
    Terrain terrain;
    private WaterSystem water;
    public RecalculatePath(Transform tran,AgentMovement m,AgentController c,Graph g,Terrain ter, WaterSystem w)
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
        Debug.Log("sono in recalculate path");
        movement.StartPathfinding();
        
        if (movement.path == null || movement.path.Count <= 0)
        {
            movement.path.Add(movement.WorldToNode(transform.position));
            GraphNode best = null;
            List<GraphNode> neighbors = (List<GraphNode>)FloodAwareAStar.GetNeighbours(graph, movement.WorldToNode(transform.position));
            float bestDist = float.MaxValue;
            foreach (var n in neighbors)
            {
                if (!n.walkable) continue; // evita acqua o ostacoli

                // distanza euclidea verso goal
                float d = Mathf.Sqrt(
                    (n.x - controller.GoalNode.x) * (n.x - controller.GoalNode.x) +
                    (n.z - controller.GoalNode.z) * (n.z - controller.GoalNode.z)
                );

                if (d < bestDist)
                {
                    bestDist = d;
                    best = n;
                }
            }
            movement.path.Add(best);
        }
    }
}