using UnityEngine;

public class WaterSystem : MonoBehaviour
{
    [Header("Fixed Water Oscillation")]
    public float minHeight = 0.5f;   // minimo desiderato
    public float maxHeight = 7f;     // massimo desiderato

    //per 0,5m/s circa 26 secondi ogni oscillazione
    // -> T = 2pi / w -> w = 2pi / T -> w = 2pi / 26 =circa 0.24166
    private float speed = 0.24166f;       // velocità oscillazione

    private float baseHeight;
    private float amplitude;

    /* update walkability */
    public Graph graph;
    public float waterOffset = 0.1f; // piccolo margine di sicurezza

    void Start()
    {
        // calcolo i valori automaticamente
        baseHeight = (minHeight + maxHeight) / 2f;          // centro
        amplitude  = (maxHeight - minHeight) / 2f;          // ampiezza

    }

    void Update()
    {
        if (graph == null) return;
        float oscillation = Mathf.Sin(Time.time * speed) * amplitude;

        float newY = baseHeight + oscillation;

        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        UpdateGraphWalkability();
    }
    public float GetWaterHeightNow()
    {
        return baseHeight + Mathf.Sin(Time.time * speed) * amplitude;
    }

    public float GetWaterHeightAt(float timeFromNow)
    {
        float futureTime = Time.time + timeFromNow;
        return baseHeight + Mathf.Sin(futureTime * speed) * amplitude;
    }
    public void SetGraph(Graph g)
    {
        graph = g;
    }
    private void UpdateGraphWalkability()
    {
        float waterH = transform.position.y;

        for (int z = 0; z < graph.height; z++)
        {
            for (int x = 0; x < graph.width; x++)
            {
                GraphNode n = graph.nodes[z, x];

                if (n.height < waterH - waterOffset)
                    n.walkable = false;
                else
                    n.walkable = true;
            }
        }
    }
}