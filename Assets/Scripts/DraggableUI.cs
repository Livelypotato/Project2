using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DraggableUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Canvas canvas; // 引用Canvas以获取正确的缩放比例
    [SerializeField] private RectTransform rectTransform; // UI元素的RectTransform
    [SerializeField] private bool returnToOriginalPosition = false; // 是否在拖拽结束后返回原位
    
    private Vector2 originalPosition; // 原始位置
    private Vector2 initialPointerPosition; // 初始鼠标位置
    private Vector2 initialAnchoredPosition; // 初始UI位置

    private void Awake()
    {
        // 如果没有在Inspector中设置，自动获取组件
        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();
        
        if (canvas == null)
            canvas = GetComponentInParent<Canvas>();
            
        // 保存原始位置
        originalPosition = rectTransform.anchoredPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 记录初始鼠标位置和UI位置
        initialPointerPosition = eventData.position;
        initialAnchoredPosition = rectTransform.anchoredPosition;
    }
    public void OnDrag(PointerEventData eventData)
    {
        // 计算鼠标移动的距离
        Vector2 pointerDelta = eventData.position - initialPointerPosition;
        
        // 根据Canvas的渲染模式调整移动距离
        if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            // 直接使用鼠标移动距离
            rectTransform.anchoredPosition = initialAnchoredPosition + pointerDelta / canvas.scaleFactor;
        }
        else if (canvas.renderMode == RenderMode.ScreenSpaceCamera || canvas.renderMode == RenderMode.WorldSpace)
        {
            // 将屏幕空间的鼠标移动转换为Canvas空间
            Vector2 startLocalPoint, endLocalPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                initialPointerPosition,
                canvas.worldCamera,
                out startLocalPoint);
                
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                eventData.position,
                canvas.worldCamera,
                out endLocalPoint);
                
            // 计算本地空间中的移动距离
            Vector2 localDelta = endLocalPoint - startLocalPoint;
            rectTransform.anchoredPosition = initialAnchoredPosition + localDelta;
        }
    }
    
    public void OnEndDrag(PointerEventData eventData)
    {
        // 如果设置了返回原位，则在拖拽结束后返回原始位置
        if (returnToOriginalPosition)
        {
            rectTransform.anchoredPosition = originalPosition;
        }
}
    
    // 重置位置的公共方法，可以从外部调用
    public void ResetPosition()
    {
        rectTransform.anchoredPosition = originalPosition;
    }
}