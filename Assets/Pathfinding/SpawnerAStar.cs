public List<GridNode> FindPathToHighPoint(GridNode[,] graph, GridNode start, float minHeight)
{
    int w = graph.GetLength(1);
    int h = graph.GetLength(0);

    List<GridNode> open = new List<GridNode>();
    HashSet<GridNode> closed = new HashSet<GridNode>();

    open.Add(start);

    while (open.Count > 0)
    {
        // Nodo con costo più basso
        GridNode current = open[0];
        for (int i = 1; i < open.Count; i++)
        {
            if (open[i].fCost < current.fCost)
                current = open[i];
        }

        open.Remove(current);
        closed.Add(current);

        // TARGET: qualsiasi nodo sufficientemente alto
        if (current.height >= minHeight)
        {
            return RetracePath(start, current);
        }

        // Esplora vicini
        foreach (GridNode neighbour in GetNeighbours(graph, current))
        {
            if (!neighbour.walkable || closed.Contains(neighbour))
                continue;

            float tentativeG = current.gCost + 1f;

            if (tentativeG < neighbour.gCost || !open.Contains(neighbour))
            {
                neighbour.gCost = tentativeG;
                neighbour.hCost = heuristic(neighbour, start); // non importa troppo qui
                neighbour.parent = current;

                if (!open.Contains(neighbour))
                    open.Add(neighbour);
            }
        }
    }

    return null; // nessun punto trovato
}

private float heuristic(GridNode a, GridNode b)
{
    return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.z - b.z);
}