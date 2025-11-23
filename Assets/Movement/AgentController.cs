using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class AgentController : MonoBehaviour
{
    public static Vector3 GoalPosition;

    // Non toccare la posizione dell’agente.
    // PerlinWalk è l’unico che lo piazza correttamente.

    void Start()
    {
        Debug.Log("[AgentController] Agent initial position = " + transform.position);
        Debug.Log("[AgentController] Goal = " + GetGoal());
    }

    public void SetGoalFromSpawn(GraphNode spawnNode, Terrain t)
    {
        int res = t.terrainData.heightmapResolution;

        int maxX = res - 1;
        int maxZ = res - 1;

        // coordinate grid del goal
        int goalX = maxX - spawnNode.x;
        int goalZ = maxZ - spawnNode.z;

        // conversione grid → world
        float worldX = ((goalX + 0.5f) / (float)(res - 1)) * t.terrainData.size.x;
        float worldZ = ((goalZ + 0.5f) / (float)(res - 1)) * t.terrainData.size.z;

        // altezza del goal (non abbiamo graph qui → raycast)
        float worldY = 0f;
        if (Physics.Raycast(new Vector3(worldX, 200f, worldZ), Vector3.down, out RaycastHit hit, Mathf.Infinity))
        {
            worldY = hit.point.y;
        }

        GoalPosition = new Vector3(worldX, worldY, worldZ);
        CreateGoalBeam(GoalPosition);
        Debug.Log($"[AgentController] Goal automatico impostato in {GoalPosition}");
    }

   private void CreateGoalBeam(Vector3 pos)
    {
        // crea un oggetto vuoto
        GameObject beam = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        beam.name = "GoalBeam";

        // altezza del fascio (più è alto, più si vede)
        float beamHeight = 30f;

        // scalalo come un raggio
        beam.transform.localScale = new Vector3(1.3f, beamHeight / 2f, 1.3f);

        // posizionalo sopra il goal
        beam.transform.position = pos + Vector3.up * (beamHeight / 2f);

        // inclina e restringi la base per sembrare un raggio di luce
        beam.transform.localScale = new Vector3(0.6f, beamHeight / 2, 0.6f);
        beam.transform.Rotate(Vector3.up * Time.deltaTime * 20f);
        // aggiungi materiale trasparente luminoso
        var renderer = beam.GetComponent<MeshRenderer>();
        renderer.material = Resources.Load<Material>("GoalBeamMat");

        // toglie il collider
        Destroy(beam.GetComponent<Collider>());
    }

    public Vector3 GetGoal()
    {
        return GoalPosition;
    }
    public void SetGoal(Vector3 g)
    {
        GoalPosition = g;
    }
}

