using UnityEngine;

public class WaterSystem : MonoBehaviour
{
    [Header("Fixed Water Oscillation")]
    public float minHeight = 0.5f;   // minimo desiderato
    public float maxHeight = 7f;     // massimo desiderato
    public float speed = 0.5f;       // velocità oscillazione

    private float baseHeight;
    private float amplitude;

    void Start()
    {
        // calcolo i valori automaticamente
        baseHeight = (minHeight + maxHeight) / 2f;          // centro
        amplitude  = (maxHeight - minHeight) / 2f;          // ampiezza
    }

    void Update()
    {
        float oscillation = Mathf.Sin(Time.time * speed) * amplitude;

        float newY = baseHeight + oscillation;

        transform.position = new Vector3(
            transform.position.x,
            newY,
            transform.position.z
        );
    }
}