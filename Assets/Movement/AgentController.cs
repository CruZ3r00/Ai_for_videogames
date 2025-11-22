using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]

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

    public Vector3 GetGoal()
    {
        return GoalPosition;
    }
    public void SetGoal(Vector3 g)
    {
        GoalPosition = g;
    }
}

