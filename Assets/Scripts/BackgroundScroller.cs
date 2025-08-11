using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    public Transform player;          // 玩家 Transform
    public float scrollFactorX = 0.5f; // X 方向滚动比例
    public float scrollFactorY = 0.5f; // Y 方向滚动比例

    private Vector3 lastPlayerPos;    // 上一帧玩家位置
    private Vector2 offset;           // 材质偏移
    private Material mat;

    void Start()
    {
        mat = GetComponent<Renderer>().material;
        if (player != null)
        {
            lastPlayerPos = player.position;
        }
    }

    void Update()
    {
        if (player == null) return;

        // 玩家本帧的位移
        Vector3 delta = player.position - lastPlayerPos;

        // 按比例更新背景贴图偏移
        offset.x += delta.x * scrollFactorX;
        offset.y += delta.y * scrollFactorY;

        // 设置材质偏移
        mat.SetTextureOffset("_MainTex", offset);

        // 更新上一帧玩家位置
        lastPlayerPos = player.position;
    }
}