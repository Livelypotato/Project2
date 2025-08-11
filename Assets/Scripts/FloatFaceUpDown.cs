using UnityEngine;

public class FloatFaceUpDown : MonoBehaviour
{
    public float floatAmplitude = 0.1f; 
    public float floatSpeed = 1f;       

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.localPosition;
    }

    void Update()
    {
        transform.localPosition = startPos + Vector3.up * Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
    }
}
