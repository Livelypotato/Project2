using UnityEngine;

public class ButtonsUIManager : MonoBehaviour
{
    [Header("按钮1 UI")]
    public GameObject button1NormalUI;
    public GameObject button1PressedUI;

    [Header("按钮2 UI")]
    public GameObject button2NormalUI;
    public GameObject button2PressedUI;

    // 切换所有按钮到“按下”状态UI
    public void ShowPressedUI()
    {
        button1NormalUI.SetActive(false);
        button1PressedUI.SetActive(true);

        button2NormalUI.SetActive(false);
        button2PressedUI.SetActive(true);
    }

    // 切换回正常状态UI
    public void ShowNormalUI()
    {
        button1NormalUI.SetActive(true);
        button1PressedUI.SetActive(false);

        button2NormalUI.SetActive(true);
        button2PressedUI.SetActive(false);
    }
}