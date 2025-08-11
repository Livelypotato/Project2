using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ClickToFillProgress : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Slider progressSlider;
    [SerializeField] private float increment = 0.1f;
    [SerializeField] private float decrement = 0.1f;
    [SerializeField] private float longPressIncrement = 0.05f; // 长按时每帧增加的量
    [SerializeField] private ButtonsUIManager uiManager;

    private float progress = 0f;
    private bool isPressed = false;

    public void ClickButton()
    {
        if (progress < 1f)
        {
            progress += increment;
            progressSlider.value = progress;
        }
    }

    void Update()
    {
        // 如果按钮被长按，持续增加进度
        if (isPressed && progress < 1f)
        {
            progress += longPressIncrement * Time.deltaTime;
            if (progress > 1f) progress = 1f;
        }
        // 否则自动减少进度
        else if (progress > 0f)
        {
            progress -= Time.deltaTime * decrement;
            if (progress < 0f) progress = 0f;
        }

        progressSlider.value = progress;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
        if (uiManager != null)
            uiManager.ShowPressedUI();
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
        if (uiManager != null)
            uiManager.ShowNormalUI();
    }
}
