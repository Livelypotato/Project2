using UnityEngine;
using UnityEngine.UI;

public class Close : MonoBehaviour
{
    [Header("Progress Settings")]
    public int clicksToComplete = 10; // 完成所需点击次数，可在Inspector中调整
    private int currentClicks = 0;

    [Header("UI Elements")]
    public Image progressArc; // 外圈环形进度条，建议使用线条状环形Sprite，类型设为Filled Radial 360
    public Image centerIcon;  // 可选：内圆图标或静态装饰

    [Header("Optional Settings")]
    public AudioSource clickSound;
    public AudioSource completeSound;

    void Start()
    {
        UpdateProgress();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnClick();
        }
    }

    public void OnClick()
    {
        currentClicks++;
        if (clickSound != null) clickSound.Play();

        if (currentClicks >= clicksToComplete)
        {
            currentClicks = clicksToComplete;
            if (completeSound != null) completeSound.Play();
            OnProgressComplete();
        }

        UpdateProgress();
    }

    void UpdateProgress()
    {
        if (progressArc != null)
        {
            float fillAmount = (float)currentClicks / clicksToComplete;
            progressArc.fillAmount = fillAmount;
        }
    }

    void OnProgressComplete()
    {
        Debug.Log("Progress Complete!");
        // 可扩展：播放动画、奖励、场景切换等
    }

    public void ResetProgress()
    {
        currentClicks = 0;
        UpdateProgress();
    }
}

/*
使用说明（整合版本）：

1. Canvas层级结构建议：

Canvas
└── ProgressArc (Image, 外圈环形)
    └── CenterIcon (Image, 内部静态装饰图，可选)

2. ProgressArc 设置：
- Source Image: 使用圆环形透明图（空心圆）
- Image Type: Filled
- Fill Method: Radial 360
- Fill Origin: Top / Left / Bottom / Right
- Clockwise: 按需
- Fill Amount: 默认 0（脚本控制）

3. CenterIcon（可选）：
- 放置游戏主图标或中央装饰内容，保持不随进度改变

4. 脚本挂载与设置：
- 挂载 ClickProgress 到空物体上（或 ProgressArc 本身）
- 拖入 ProgressArc 和 CenterIcon
- 设置点击数（clicksToComplete）和音效组件（可选）

5. 运行时点击任意区域，外圈进度条将线性增长至满圈后触发完成事件。
*/
