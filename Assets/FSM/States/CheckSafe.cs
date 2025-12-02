using UnityEngine;
using System.Collections;
using Unity.VisualScripting.FullSerializer;



public class CheckSafe
{
    Transform transform;
    AgentMovement movement;
    AgentController controller;
    Graph graph;
    Terrain terrain;
    private WaterSystem water;

    public bool isSafe = false;
    
    public bool recalc = false;
    public CheckSafe(Transform tran,AgentMovement m,AgentController c,Graph g,Terrain ter, WaterSystem w)
    {
        transform = tran;
        movement = m;
        controller = c;
        graph = g;
        terrain = ter;
        water = w;
    }
    
    public void Enter()
    {
        Debug.Log("sono in check safe");
        int i = movement.currentIndex;
        isSafe = false;
        recalc = false;
        if (movement.path == null || movement.path.Count == 0)
        {
            recalc = true;
            // nessun path = non safe / forza ricalcolo
            return;
        }

        while (i < movement.path.Count && movement.EstimateTimeToReach(i) < 8f)
        {
            float arrivalTime = movement.EstimateTimeToReach(i);
            float waterAtArrival = water.GetWaterHeightAt(arrivalTime);

            float nodeHeight = movement.path[i].height;

            if (nodeHeight < waterAtArrival)
            {
                // questo nodo sarà sommerso quando l’agente ci arriva
                isSafe = false;
                return;
            }
            i++;
        }

        isSafe = true; // tutti i nodi futuri sono più alti dell’acqua
    }
}