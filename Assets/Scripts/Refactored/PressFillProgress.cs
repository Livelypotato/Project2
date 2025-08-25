using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PressFillProgress : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public enum TargetType { Image, Slider }

    [Header("Target")]
    public TargetType targetType = TargetType.Slider;
    public Image targetImage;
    public Slider targetSlider;

    [Header("Behavior")]
    public bool enableClickAccumulation = true; // 是否允许通过外部按钮点击叠加
    public bool enableAutoDecay = true;         // 松开后是否自动衰减

    [Header("Speeds")]
    public float clickIncrement = 0.1f; // 每次点击增加
    public float holdSpeed = 0.5f;      // 长按每秒增加
    public float decaySpeed = 0.3f;     // 每秒衰减

    private float _p;
    private bool _holding;

    void Update()
    {
        if (_holding)
        {
            _p += holdSpeed * Time.deltaTime;
        }
        else if (enableAutoDecay)
        {
            _p -= decaySpeed * Time.deltaTime;
        }
        _p = Mathf.Clamp01(_p);
        Flush();
    }

    public void IncrementOnce()
    {
        if (!enableClickAccumulation) return;
        _p = Mathf.Clamp01(_p + clickIncrement);
        Flush();
    }

    public void ResetProgress()
    {
        _p = 0f;
        Flush();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _holding = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _holding = false;
    }

    private void Flush()
    {
        switch (targetType)
        {
            case TargetType.Image:
                if (targetImage != null)
                {
                    targetImage.type = Image.Type.Filled;
                    targetImage.fillAmount = _p;
                }
                break;
            case TargetType.Slider:
                if (targetSlider != null)
                {
                    targetSlider.value = _p;
                }
                break;
        }
    }
} 