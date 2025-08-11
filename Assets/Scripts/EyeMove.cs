using UnityEngine;

public class EyeMove : MonoBehaviour
{
    public float moveAmplitude = 0.05f; // 左右移动幅度
    public float moveSpeed = 1f;        // 左右移动速度

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.localPosition;
    }

    void Update()
    {
        transform.localPosition = startPos + Vector3.right * Mathf.Sin(Time.time * moveSpeed) * moveAmplitude;
    }
}
