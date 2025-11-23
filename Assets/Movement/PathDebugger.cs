using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class PathDebugger : MonoBehaviour
{
    LineRenderer lr;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        lr.startWidth = 0.2f;
        lr.endWidth = 0.2f;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.positionCount = 0;
        lr.startColor = Color.yellow;
        lr.endColor = Color.red;
    }

    public void ShowPath(List<Vector3> worldPositions)
    {
        if (worldPositions == null || worldPositions.Count == 0)
        {
            lr.positionCount = 0;
            return;
        }

        lr.positionCount = worldPositions.Count;

        for (int i = 0; i < worldPositions.Count; i++)
        {
            lr.SetPosition(i, worldPositions[i]);
        }
    }
}