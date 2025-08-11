using UnityEngine;

public class PupilMove : MonoBehaviour
{
    public float pupilAmplitude = 0.02f; // 左右移动幅度
    public float pupilSpeed = 1.5f;      // 左右移动速度

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.localPosition;
    }

    void Update()
    {
        transform.localPosition = startPos + Vector3.right * Mathf.Sin(Time.time * pupilSpeed) * pupilAmplitude;
    }
}
