using System.Collections.Generic;
using UnityEngine;

public class AgentMovement : MonoBehaviour
{
    public Terrain terrain;
    public Graph graph;

    private AgentController controller;
    private List<GraphNode> path;
    private int currentIndex = 0;

    public float flatSpeed = 1f;
    public float uphillSpeed = 0.5f;

    public void Initialize(Graph g, Terrain t)
    {
        graph = g;
        terrain = t;
        controller = GetComponent<AgentController>();
    }

    public void StartPathfinding()
    {
        if (graph == null || terrain == null || controller == null)
        {
            Debug.LogError("[AgentMovement] Missing references.");
            return;
        }

        GraphNode startNode = WorldToNode(transform.position);
        GraphNode goalNode  = controller.GoalNode;

        path = PathfinderAStar.FindFullPath(graph, startNode, goalNode);

        if (path == null || path.Count == 0)
        {
            Debug.LogError("[AgentMovement] PATH NON TROVATO!");
            return;
        }

        currentIndex = 0;
        Debug.Log("[AgentMovement] Path trovato. Lunghezza: " + path.Count);

        // debug visuale
        var dbg = GetComponent<PathDebugger>();
        if (dbg != null)
        {
            List<Vector3> worldPath = new List<Vector3>();
            foreach (var n in path)
                worldPath.Add(NodeToWorld(n));
            dbg.ShowPath(worldPath);
        }
    }

    void Update()
    {
        if (path == null || currentIndex >= path.Count)
            return;

        GraphNode nextNode = path[currentIndex];
        Vector3 targetPos = NodeToWorld(nextNode);

        Vector3 pos = transform.position;

        // movimento solo in XZ
        Vector3 flatPos    = new Vector3(pos.x, 0, pos.z);
        Vector3 flatTarget = new Vector3(targetPos.x, 0, targetPos.z);

        Vector3 toTarget = flatTarget - flatPos;
        float dist = toTarget.magnitude;

        if (dist < 0.1f)
        {
            currentIndex++;
            return;
        }

        Vector3 dir = toTarget / dist;

        float speed = GetSpeedBetween(
            currentIndex > 0 ? path[currentIndex - 1] : nextNode,
            nextNode
        );

        Vector3 step = dir * speed * Time.deltaTime;

        Vector3 newPos = new Vector3(
            pos.x + step.x,
            pos.y,
            pos.z + step.z
        );

        float y = terrain.SampleHeight(newPos) + 0.1f;
        transform.position = new Vector3(newPos.x, y, newPos.z);

        Vector3 goalWorld = controller.GetGoal();
        Vector3 flatGoal = new Vector3(goalWorld.x, 0, goalWorld.z);
        flatPos  = new Vector3(transform.position.x, 0, transform.position.z);

        if (Vector3.Distance(flatPos, flatGoal) < 0.8f)
        {
            OnReachedGoal();
            return;
        }
    }
    private void OnReachedGoal()
    {
        Debug.Log("Goal raggiunto! Ricalcolo nuovo goal...");

        int res = terrain.terrainData.heightmapResolution - 1;

        // posizione agente → coordinate heightmap
        int agentX = Mathf.RoundToInt((transform.position.x / terrain.terrainData.size.x) * res);
        int agentZ = Mathf.RoundToInt((transform.position.z / terrain.terrainData.size.z) * res);

        agentX = Mathf.Clamp(agentX, 0, res);
        agentZ = Mathf.Clamp(agentZ, 0, res);

        // goal opposto
        int goalX = res - agentX;
        int goalZ = res - agentZ;

        goalX = Mathf.Clamp(goalX, 0, res);
        goalZ = Mathf.Clamp(goalZ, 0, res);

        GraphNode opposite = graph.nodes[goalZ, goalX];
        GraphNode newGoal  = SpawnerAStar.FindSpawnPoint(graph, opposite, 7.5f);

        controller.SetGoal(newGoal, terrain);

        // Ricrea il path
        StartPathfinding();

        // Riparti dal nodo 0
        currentIndex = 0;
    }

    float GetSpeedBetween(GraphNode curr, GraphNode next)
    {
        return next.height > curr.height ? uphillSpeed : flatSpeed;
    }

    // ---- Conversioni ----
    GraphNode WorldToNode(Vector3 pos)
    {
        int res = terrain.terrainData.heightmapResolution;
        float tx = pos.x / terrain.terrainData.size.x;
        float tz = pos.z / terrain.terrainData.size.z;

        int x = Mathf.RoundToInt(tx * (res - 1));
        int z = Mathf.RoundToInt(tz * (res - 1));

        x = Mathf.Clamp(x, 0, res - 1);
        z = Mathf.Clamp(z, 0, res - 1);

        return graph.nodes[z, x];
    }

    Vector3 NodeToWorld(GraphNode node)
    {
        int res = terrain.terrainData.heightmapResolution - 1;

        float worldX = ((node.x + 0.5f) / res) * terrain.terrainData.size.x;
        float worldZ = ((node.z + 0.5f) / res) * terrain.terrainData.size.z;

        float worldY = terrain.SampleHeight(new Vector3(worldX, 0, worldZ));

        return new Vector3(worldX, worldY + 0.1f, worldZ);
    }
}