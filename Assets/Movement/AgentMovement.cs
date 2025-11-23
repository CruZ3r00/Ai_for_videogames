using System.Collections.Generic;
using UnityEngine;

public class AgentMovement : MonoBehaviour
{
    /*
    public Terrain terrain;
    public Graph graph;   
    public float baseSpeed = 1f;

    private AgentController agentController;
    private List<GraphNode> path;
    private int currentIndex = 0;

    void Start()
    {
        agentController = GetComponent<AgentController>();

        if (terrain == null)
        {
            terrain = FindAnyObjectByType<Terrain>();
        }

        // recupera goal da AgentController
        Vector3 goalWorld = agentController.GetGoal();

        // converte goal world → goal grid
        GraphNode startNode = WorldToNode(transform.position);
        GraphNode goalNode = WorldToNode(goalWorld);

        // pathfinding
        path = HeightmapPathfinder.FindFullPath(graph, startNode, goalNode);

        if (path == null)
        {
            Debug.LogError("PATH NON TROVATO!");
            return;
        }

        Debug.Log("Path trovato. Lunghezza: " + path.Count);
    }

    void Update()
    {
        if (path == null || currentIndex >= path.Count) return;

        // converti il nodo corrente in posizione world
        Vector3 nextPos = NodeToWorld(path[currentIndex]);

        // calcola direzione
        Vector3 dir = (nextPos - transform.position).normalized;

        // movimento
        transform.position += dir * baseSpeed * Time.deltaTime;

        // mantieni l'agente attaccato al terreno
        float y = terrain.SampleHeight(transform.position);
        transform.position = new Vector3(transform.position.x, y + 0.05f, transform.position.z);

        // controllo arrivo al nodo
        if (Vector3.Distance(transform.position, nextPos) < 0.5f)
        {
            currentIndex++;
        }
    }

    // ------------------------------------------------------------
    // CONVERSIONI GRIGLIA <-> WORLD
    // ------------------------------------------------------------

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

        return new Vector3(worldX, worldY + 0.05f, worldZ);
    }*/
}