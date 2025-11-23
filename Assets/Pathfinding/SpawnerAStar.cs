using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
        
        List<GraphNode> open = new List<GraphNode>();
        HashSet<GraphNode> closed = new HashSet<GraphNode>();

        start.gCost = 0;
        open.Add(start);

        while (open.Count > 0)
        {
            // trova nodo con fCost minore
            GraphNode current = open[0];
            for (int i = 1; i < open.Count; i++)
            {
                if (open[i].fCost < current.fCost)
                    current = open[i];
            }

            open.Remove(current);
            closed.Add(current);

            // obiettivo: altezza
            if (current.height >= minHeight)
                return current;

            // esplora vicini
            foreach (GraphNode n in GetNeighbours(graph, current))
            {
                if (!n.walkable || closed.Contains(n))
                    continue;

                float tentativeG = current.gCost + 1f;

                if (tentativeG < n.gCost)
                {
                    n.gCost = tentativeG;
                    n.hCost = Heuristic(start, n);
                    n.parent = current;

                    if (!open.Contains(n))
                        open.Add(n);
                }
            }
        }

        return null; // nessun punto alto trovato
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