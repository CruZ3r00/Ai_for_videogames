using UnityEngine;

public class AgentController : MonoBehaviour
{
    public static Vector3 GoalPosition;
    public GraphNode GoalNode;   // <-- nuovo

    void Start()
    {
        Debug.Log("[AgentController] Agent initial position = " + transform.position);
    }

    public void SetGoal(GraphNode goalNode, Terrain t)
    {
        GoalNode = goalNode;  // <-- salviamo anche il nodo di goal

        int res = t.terrainData.heightmapResolution - 1;

        float worldX = ((goalNode.x + 0.5f) / res) * t.terrainData.size.x;
        float worldZ = ((goalNode.z + 0.5f) / res) * t.terrainData.size.z;

        float worldY = t.SampleHeight(new Vector3(worldX, 0, worldZ)) + 0.05f;

        GoalPosition = new Vector3(worldX, worldY, worldZ);

        CreateGoalBeam(GoalPosition);
        Debug.Log($"[AgentController] Goal automatico impostato in {GoalPosition}");
    }

    private void CreateGoalBeam(Vector3 pos)
    {
        GameObject beam = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        beam.name = "GoalBeam";

        float beamHeight = 30f;
        beam.transform.localScale = new Vector3(0.6f, beamHeight / 2f, 0.6f);
        beam.transform.position = pos + Vector3.up * (beamHeight / 2f);

        var renderer = beam.GetComponent<MeshRenderer>();
        renderer.material = Resources.Load<Material>("GoalBeamMat");

        Destroy(beam.GetComponent<Collider>());
    }

    public Vector3 GetGoal() => GoalPosition;
}

