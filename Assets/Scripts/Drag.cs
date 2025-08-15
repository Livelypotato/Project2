using UnityEngine;

/// <summary>
/// 拖拽系统主管理器 - 管理条件检查和图标显示逻辑
/// </summary>
public class Drag : MonoBehaviour
{
    [Header("拖拽图标设置")]
    [Tooltip("可拖拽的图标数组")]
    public DraggableIcon[] draggableIcons;
    
    [Header("网格区域设置")]
    [Tooltip("网格区域数组")]
    public DropZone[] dropZones;
    
    [Header("条件显示设置")]
    [Tooltip("条件满足时要显示的图标")]
    public GameObject conditionalIcon;
    [Tooltip("icon3要显示的目标位置（grid3）")]
    public Transform targetPosition;
    
    private Transform originalParent; // 记录icon3的原始父对象
    private Vector2 originalPosition; // 记录icon3的原始位置
    
    // 条件检查规则：icon1拖入grid1 且 icon2拖入grid2 时显示icon3
    private bool CheckCondition()
    {
        // 检查是否有图标在grid1和grid2中
        bool icon1InGrid1 = dropZones.Length > 0 && dropZones[0].currentIcon != null && 
                           dropZones[0].currentIcon.iconId == 1;
        bool icon2InGrid2 = dropZones.Length > 1 && dropZones[1].currentIcon != null && 
                           dropZones[1].currentIcon.iconId == 2;
        
        return icon1InGrid1 && icon2InGrid2;
    }
    
    public void OnIconDropped()
    {
        Debug.Log("OnIconDropped: 开始检查条件");
        
        // 检查条件并控制icon3的显示和位置
        if (conditionalIcon != null)
        {
            bool conditionMet = CheckCondition();
            Debug.Log($"OnIconDropped: 条件检查结果 = {conditionMet}");
            
            if (conditionMet && targetPosition != null)
            {
                Debug.Log($"OnIconDropped: 条件满足，显示并移动 {conditionalIcon.name} 到 {targetPosition.name}");
                // 条件满足：显示icon3并移动到grid3
                conditionalIcon.SetActive(true);
                conditionalIcon.transform.SetParent(targetPosition);
                conditionalIcon.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            }
            else if (!conditionMet)
            {
                Debug.Log($"OnIconDropped: 条件不满足，隐藏 {conditionalIcon.name}");
                // 条件不满足：隐藏icon3
                conditionalIcon.SetActive(false);
                if (originalParent != null)
                {
                    conditionalIcon.transform.SetParent(originalParent);
                    conditionalIcon.GetComponent<RectTransform>().anchoredPosition = originalPosition;
                }
            }
        }
        
        Debug.Log("OnIconDropped: 完成");
    }
    
    void Start()
    {
        // 初始化时记录icon3的原始位置并隐藏
        if (conditionalIcon != null)
        {
            originalParent = conditionalIcon.transform.parent;
            originalPosition = conditionalIcon.GetComponent<RectTransform>().anchoredPosition;
            conditionalIcon.SetActive(false); // 默认隐藏icon3
        }
    }
}
