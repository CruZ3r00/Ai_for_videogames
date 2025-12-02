using System.Collections.Generic;
using System.Linq;
using UnityEngine;
/*
    simile a PathfinderAStar, ma trova solo il punto piu vicino che rispetti un'altezza minima
    dal punto di partenza "Node"
    il punto di partenza al primo avvio è 0,0,0. poi è calcolato in base a dove si trova l'agente
    Usato in PerlinWalk per lo spawn iniziale dell'agente e per il goal nell'angolo opposto
*/
public class SpawnerAStar
{
    private static readonly int[,] dirs = {
        { 1, 0 }, { -1, 0 },
        { 0, 1 }, { 0, -1 },
        { 1, 1 }, { -1, 1 },
        { 1,-1 }, { -1,-1 }
    };

    public static GraphNode FindSpawnPoint(Graph graph, GraphNode start, float minHeight)
    {
         Queue<GraphNode> open = new Queue<GraphNode>();
        HashSet<GraphNode> visited = new HashSet<GraphNode>();

        open.Enqueue(start);
        visited.Add(start);

        while (open.Count > 0)
        {
            GraphNode current = open.Dequeue();

            // obiettivo: primo nodo più vicino con altezza sufficiente
            if (current.height >= minHeight)
                return current;

            foreach (GraphNode n in GetNeighbours(graph, current))
            {
                if (!n.walkable || visited.Contains(n))
                    continue;

                visited.Add(n);
                open.Enqueue(n);
            }
        }

        return null;
    }
    
    private static IEnumerable<GraphNode> GetNeighbours(Graph graph, GraphNode node)
    {
        for (int i = 0; i < dirs.GetLength(0); i++)
        {
            int nx = node.x + dirs[i, 0];
            int nz = node.z + dirs[i, 1];

            if (nx >= 0 && nx < graph.width && nz >= 0 && nz < graph.height)
                yield return graph.nodes[nz, nx];
        }
    }

    private static float Heuristic(GraphNode a, GraphNode b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.z - b.z);
    }
}