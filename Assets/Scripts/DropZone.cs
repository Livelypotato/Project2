using UnityEngine;

/// <summary>
/// 拖放区域组件
/// </summary>
public class DropZone : MonoBehaviour
{
    [Header("拖放区域设置")]
    [Tooltip("接受的图标ID数组，空数组表示接受所有图标")]
    public int[] acceptedIconIds = new int[0]; // 空数组表示接受所有图标
    
    [HideInInspector]
    public DraggableIcon currentIcon; // 当前在此区域的图标
    
    private Drag gameManager;
    
    void Start()
    {
        gameManager = FindObjectOfType<Drag>();
    }
    
    public bool CanAcceptIcon(DraggableIcon icon)
    {
        Debug.Log($"DropZone {name} 检查图标 {icon.name} (ID: {icon.iconId})");
        Debug.Log($"当前图标: {(currentIcon != null ? currentIcon.name : "无")}");
        Debug.Log($"接受数组长度: {acceptedIconIds.Length}");
        
        // 如果已经有图标了，不能再接受新的
        if (currentIcon != null) 
        {
            Debug.Log("拒绝：已有图标");
            return false;
        }
        
        // 如果数组为空，接受所有图标
        if (acceptedIconIds.Length == 0) 
        {
            Debug.Log("接受：数组为空");
            return true;
        }
        
        // 检查图标ID是否在接受列表中
        for (int i = 0; i < acceptedIconIds.Length; i++)
        {
            if (acceptedIconIds[i] == icon.iconId)
            {
                Debug.Log("接受：ID匹配");
                return true;
            }
        }
        
        Debug.Log("拒绝：ID不匹配");
        return false; // 图标ID不在接受列表中
    }
    
    public void AcceptIcon(DraggableIcon icon)
    {
        Debug.Log($"AcceptIcon: 开始接受图标 {icon.name}");
        Debug.Log($"图标当前父对象: {icon.transform.parent.name}");
        
        // 将图标放置在此区域
        currentIcon = icon;
        icon.transform.SetParent(transform);
        icon.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        
        Debug.Log($"AcceptIcon: 图标已设置父对象为 {transform.name}");
        Debug.Log($"AcceptIcon: 图标位置设置为 {icon.GetComponent<RectTransform>().anchoredPosition}");
        
        // 通知游戏管理器检查条件
        if (gameManager != null)
        {
            Debug.Log("AcceptIcon: 通知游戏管理器");
            gameManager.OnIconDropped();
        }
        
        Debug.Log($"AcceptIcon: 完成，currentIcon = {currentIcon.name}");
    }
    
    public void RemoveIcon()
    {
        if (currentIcon != null)
        {
            currentIcon.ReturnToOriginalPosition();
            currentIcon = null;
            
            // 通知游戏管理器检查条件
            if (gameManager != null)
            {
                gameManager.OnIconDropped();
            }
        }
    }
}
