using UnityEngine;
using System.Collections.Generic;
using System.Linq;
/*
    implementazione dell'algoritmo A* sulla griglia Graph di GraphNode
    dato start e goal restituisce lista ordinata dei nodi da seguire per raggiungere il target
    utilizzato da AgentMovement e utilizza Graph e GraphNode
*/
public class PathfinderAStar 
{
    //Vettori di offset (8 direzioni: 4 ortogonali + 4 diagonali).
    private static readonly int[,] dirs = {
        { 1, 0 }, { -1, 0 },
        { 0, 1 }, { 0, -1 },
        { 1, 1 }, { -1, 1 },
        { 1,-1 }, { -1,-1 }
    };

    //calcola il path
    public static List<GraphNode> FindFullPath(Graph graph, GraphNode start, GraphNode goal)
    {
        //nodi da valutare
        List<GraphNode> open = new List<GraphNode>();
        //nodi valutati
        HashSet<GraphNode> closed = new HashSet<GraphNode>();

        //reset di tutti i costi tra i nodi prima di iniziare
        foreach (var node in graph.nodes)
        {
            node.gCost = Mathf.Infinity;
            node.hCost = 0;
            node.parent = null;
        }

        start.gCost = 0;
        start.hCost = Heuristic(start, goal);

        open.Add(start);

        //loop fino a quando ci sono nodi da valutare
        while (open.Count > 0)
        {
            //cerco il nodo con costo minore
            GraphNode current = open[0];
            for (int i = 1; i < open.Count; i++)
            {
                if (open[i].fCost < current.fCost)
                    current = open[i];
            }

            //sposto il current node nella lista dei nodi da valutare
            open.Remove(current);
            closed.Add(current);

            //se il current è il goal allora siamo arrivati
            if (current == goal)
                //restituisco il path al contrario (ordine giusto)
                return RetracePath(start, goal);

            //ciclo tutti i vicini
            foreach (GraphNode n in GetNeighbours(graph, current))
            {   
                //se non camminabili o contenuti in close skippo
                if (!n.walkable || closed.Contains(n))
                    continue;

                //costo ipotetico per arrivare al prossimo nodo (n)
                float tentativeG = current.gCost + 1f;

                //se il percorso è migliore lo seleziono
                if (tentativeG < n.gCost)
                {
                    n.gCost = tentativeG;
                    n.hCost = Heuristic(n, goal);
                    n.parent = current;

                    if (!open.Contains(n))
                        open.Add(n);
                }
            }
        }

        return null;
    }

    //dal goal fino allo start segue il percorso tramite i nodi parent impostati durante l"A* e inverte la lista
    private static List<GraphNode> RetracePath(GraphNode start, GraphNode end)
    {
        List<GraphNode> path = new List<GraphNode>();
        GraphNode current = end;

        // Risale i parent dal goal fino allo start
        while (current != start)
        {
            path.Add(current);
            current = current.parent;
        }

        // Aggiungi lo start (opzionale: dipende se vuoi includere il nodo di origine)
        path.Add(start);

        // Inverti la lista per ottenere start → ... → goal
        path.Reverse();

        return path;
    }

    //restituisce tutti i vicini del nodo in input
    public static IEnumerable<GraphNode> GetNeighbours(Graph graph, GraphNode node)
    {
        for (int i = 0; i < dirs.GetLength(0); i++)
        {
            int nx = node.x + dirs[i, 0];
            int nz = node.z + dirs[i, 1];

            if (nx >= 0 && nx < graph.width && nz >= 0 && nz < graph.height)
                yield return graph.nodes[nz, nx];
        }
    }

    //euristica di manhattan
    private static float Heuristic(GraphNode a, GraphNode b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.z - b.z);
    }
}