using UnityEngine;

public class SinWaveMover : MonoBehaviour
{
    [Header("Motion")]
    public Vector3 direction = Vector3.right; // 方向
    public float amplitude = 0.1f;            // 幅度
    public float speed = 1f;                  // 速度
    public bool useLocalSpace = true;         // 是否在局部空间移动

    private Vector3 _origin;

    void Start()
    {
        _origin = useLocalSpace ? transform.localPosition : transform.position;
    }

    void Update()
    {
        Vector3 offset = direction.normalized * Mathf.Sin(Time.time * speed) * amplitude;
        if (useLocalSpace)
            transform.localPosition = _origin + offset;
        else
            transform.position = _origin + offset;
    }
}