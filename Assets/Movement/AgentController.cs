using UnityEngine;
/*
    memorizzazione del goal corrente sia come nodo che come posizione nel mondo, 
    creazione del fascio di luce del target, get del goal per gli altri componenti
*/
public class AgentController : MonoBehaviour
{
    public static Vector3 GoalPosition;
    public GraphNode GoalNode;   // <-- nuovo

    void Start()
    {
        Debug.Log("[AgentController] Agent initial position = " + transform.position);
    }

    //funzione che setta il goal sia come nodo che come posizione
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

    //creazuibe del fascio di luce ed eliminazione del precedente se dovesse essercene gia uno
    private void CreateGoalBeam(Vector3 pos)
    {
        // 1) Rimuovi eventuale fascio esistente
        GameObject oldBeam = GameObject.Find("GoalBeam");
        if (oldBeam != null)
            Destroy(oldBeam);

        // 2) Crea il nuovo fascio
        GameObject beam = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        beam.name = "GoalBeam";

        float beamHeight = 30f;
        beam.transform.localScale = new Vector3(0.6f, beamHeight / 2f, 0.6f);

        beam.transform.position = pos + Vector3.up * (beamHeight / 2f);

        // materiale
        var renderer = beam.GetComponent<MeshRenderer>();
        renderer.material = Resources.Load<Material>("GoalBeamMat");

        // togli il collider
        Destroy(beam.GetComponent<Collider>());
    }

    public Vector3 GetGoal() => GoalPosition;
}

