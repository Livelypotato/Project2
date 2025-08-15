using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

/// <summary>
/// 可拖拽的图标组件
/// </summary>
public class DraggableIcon : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("图标设置")]
    public int iconId; // 图标ID，用于识别不同的图标
    
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Vector2 originalPosition;
    private Transform originalParent;
    
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        canvas = FindObjectOfType<Canvas>();
    }
    
    void Start()
    {
        originalPosition = rectTransform.anchoredPosition;
        originalParent = transform.parent;
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log($"开始拖拽 {name} (ID: {iconId})");
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
        
        // 如果当前在某个DropZone中，先清理该区域的引用
        DropZone currentDropZone = GetComponentInParent<DropZone>();
        if (currentDropZone != null)
        {
            Debug.Log($"清理 {currentDropZone.name} 的当前图标引用");
            currentDropZone.currentIcon = null;
        }
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }
    
    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        
        // 检查是否放在了有效的拖放区域
        bool droppedInValidZone = false;
        
        // 通过射线检测找到拖放区域
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        
        Debug.Log($"射线检测到 {results.Count} 个对象");
        foreach (RaycastResult result in results)
        {
            Debug.Log($"检测到: {result.gameObject.name}");
        }
        
        foreach (RaycastResult result in results)
        {
            DropZone dropZone = result.gameObject.GetComponent<DropZone>();
            if (dropZone != null)
            {
                Debug.Log($"找到DropZone: {dropZone.name}");
                if (dropZone.CanAcceptIcon(this))
                {
                    Debug.Log($"准备放置到 {dropZone.name}");
                    dropZone.AcceptIcon(this);
                    droppedInValidZone = true;
                    Debug.Log($"放置完成，droppedInValidZone = {droppedInValidZone}");
                    break;
                }
                else
                {
                    Debug.Log($"{dropZone.name} 不能接受此图标");
                }
            }
        }
        
        // 如果没有放在有效区域，返回原位置
        if (!droppedInValidZone)
        {
            Debug.Log("未找到有效区域，返回原位置");
            ReturnToOriginalPosition();
            
            // 通知游戏管理器检查条件（图标被移除）
            Drag gameManager = FindObjectOfType<Drag>();
            if (gameManager != null)
            {
                gameManager.OnIconDropped();
            }
        }
        else
        {
            Debug.Log("成功放置在有效区域");
        }
    }
    
    public void ReturnToOriginalPosition()
    {
        transform.SetParent(originalParent);
        rectTransform.anchoredPosition = originalPosition;
    }
}
