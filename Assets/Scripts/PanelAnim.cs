using System.Collections;
using UnityEngine;

public class PanelAnim : MonoBehaviour
{
    public AnimationCurve showCurve;   // 显示曲线
    public AnimationCurve hideCurve;   // 隐藏曲线
    public float animationSpeed = 1f;  // 动画速度
    public RectTransform panel;        // UI面板 (注意要用RectTransform)

    // 面板的目标位置（右上角）
    public Vector2 targetPos = new Vector2(300, -100);

    // 面板隐藏时的初始位置（屏幕左边外侧）
    public Vector2 offscreenPos = new Vector2(-500, -100);

    // 停留时间（秒）
    public float stayTime = 2f;

    private void Start()
    {
        // 初始在左边隐藏
        panel.anchoredPosition = offscreenPos;
    }

    public void ShowAchievement()
    {
        StopAllCoroutines();
        StartCoroutine(ShowAndHideRoutine());
    }

    IEnumerator ShowAndHideRoutine()
    {
        // 显示动画
        yield return StartCoroutine(ShowPanel());

        // 停留几秒
        yield return new WaitForSeconds(stayTime);

        // 隐藏动画
        yield return StartCoroutine(HidePanel());
    }

    IEnumerator ShowPanel()
    {
        float timer = 0;
        while (timer < 1)
        {
            float t = showCurve.Evaluate(timer);
            panel.anchoredPosition = Vector2.Lerp(offscreenPos, targetPos, t);
            timer += Time.deltaTime * animationSpeed;
            yield return null;
        }
        panel.anchoredPosition = targetPos;
    }

    IEnumerator HidePanel()
    {
        float timer = 0;
        while (timer < 1)
        {
            float t = hideCurve.Evaluate(timer);
            panel.anchoredPosition = Vector2.Lerp(targetPos, offscreenPos, t);
            timer += Time.deltaTime * animationSpeed;
            yield return null;
        }
        panel.anchoredPosition = offscreenPos;
    }
}