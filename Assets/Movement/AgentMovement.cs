using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class AgentMovement : MonoBehaviour
{
    public Terrain terrain;
    public Graph graph;

    private AgentController controller;
    private CharacterController cc;

    private List<GraphNode> path;
    private int currentIndex = 0;

    public float gravity = 10f;

    public void Initialize(Graph g, Terrain t, Vector3 startPos)
    {
        graph = g;
        terrain = t;

        cc = GetComponent<CharacterController>();

        transform.position = startPos;

        StartPathfinding();
    }

    void StartPathfinding()
    {
        controller = GetComponent<AgentController>();

        Vector3 goalWorld = controller.GetGoal();

        GraphNode startNode = WorldToNode(transform.position);
        GraphNode goalNode  = WorldToNode(goalWorld);

        path = PathfinderAStar.FindFullPath(graph, startNode, goalNode);

        if (path == null)
        {
            Debug.LogError("PATH NON TROVATO!");
            return;
        }

        Debug.Log("Path trovato. Lunghezza: " + path.Count);
        var debug = GetComponent<PathDebugger>();
        if (debug != null)
        {
            List<Vector3> wp = new List<Vector3>();
            foreach (GraphNode n in path)
                wp.Add(NodeToWorld(n));

            debug.ShowPath(wp);
        }
    }

    void Update()
    {
        if (path == null || currentIndex >= path.Count)
            return;

        // prossimo nodo reale
        GraphNode nextNode = currentIndex < path.Count - 1
                            ? path[currentIndex + 1]
                            : path[currentIndex];

        Vector3 targetPos = NodeToWorld(nextNode);

        Vector3 curr = transform.position;

        Vector3 flatCurr   = new Vector3(curr.x, 0, curr.z);
        Vector3 flatTarget = new Vector3(targetPos.x, 0, targetPos.z);

        Vector3 dirXZ = (flatTarget - flatCurr).normalized;

        // se la direzione è zero... NON muove. Risolvi così:
        if (dirXZ == Vector3.zero)
            return;

        // velocità
        float speed = GetSpeedBetween(
            currentIndex > 0 ? path[currentIndex - 1] : nextNode,
            nextNode
        );

        Vector3 move = dirXZ * speed;

        // forza verticale controllata
        float terrainY = terrain.SampleHeight(curr);
        float desiredY = terrainY + 0.1f;

        float deltaY = desiredY - curr.y;

        move.y = deltaY * 5f - 1.5f;

        cc.Move(move * Time.deltaTime);

        // passo nodo?
        if (Vector3.Distance(flatCurr, flatTarget) < 0.5f)
            currentIndex++;
    }

    float GetSpeedBetween(GraphNode current, GraphNode next)
    {
        return next.height > current.height ? 0.5f : 1f;
    }

    GraphNode WorldToNode(Vector3 pos)
    {
        int res = terrain.terrainData.heightmapResolution;

        float tx = pos.x / terrain.terrainData.size.x;
        float tz = pos.z / terrain.terrainData.size.z;

        int x = Mathf.Clamp(Mathf.RoundToInt(tx * (res - 1)), 0, res - 1);
        int z = Mathf.Clamp(Mathf.RoundToInt(tz * (res - 1)), 0, res - 1);

        return graph.nodes[z, x];
    }

    Vector3 NodeToWorld(GraphNode node)
    {
        int res = terrain.terrainData.heightmapResolution;

        float worldX = ((node.x + 0.5f) / (res - 1)) * terrain.terrainData.size.x;
        float worldZ = ((node.z + 0.5f) / (res - 1)) * terrain.terrainData.size.z;

        float worldY = terrain.SampleHeight(new Vector3(worldX, 0, worldZ));

        return new Vector3(worldX, worldY, worldZ);
    }
}