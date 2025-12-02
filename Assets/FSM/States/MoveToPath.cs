using UnityEngine;
using System.Collections;
using Unity.VisualScripting.FullSerializer;



public class MoveToPath
{
    Transform transform;
    AgentMovement movement;
    AgentController controller;
    Graph graph;
    Terrain terrain;
    private WaterSystem water;
    public MoveToPath(Transform tran,AgentMovement m,AgentController c,Graph g,Terrain ter, WaterSystem w)
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
        Debug.Log("sono in move to path");
    }

    public void Exit()
    {
    }

}