using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target;     // l'agente o il CameraPivot
    public float smoothSpeed = 10f;
    public float rotationSpeed = 5f;

    private Vector3 offset;

    void Start()
    {
        offset = transform.position - target.position;
    }

    void LateUpdate()
    {
        // --- SMOOTH FOLLOW ---
        Vector3 desiredPos = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPos, smoothSpeed * Time.deltaTime);

        // --- CAMERA LOOKS AT TARGET ---
        Quaternion targetRot = Quaternion.LookRotation(target.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
    }
}
