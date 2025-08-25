using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class DialogTriggerMiniGame : MonoBehaviour
{
    public GameObject dialogPanel;          // 对话框UI整体
    public TMP_Text dialogTMPText;          // 对话文字
    [TextArea(3, 10)]
    public string message;                  // 要显示的文字内容
    public float typingSpeed = 0.05f;       // 打字速度

    public SpriteRenderer eKeyPrompt;       // E键提示SpriteRenderer
    public GameObject miniGameRoot;         // ⬅ 拖入小游戏整体父物体
    public GameObject mainSceneRoot;        // ⬅ 主场景内容父物体(可选)

    private bool playerInRange = false;
    private Coroutine typingCoroutine;

    void Awake()
    {
        dialogPanel.SetActive(false);
        miniGameRoot.SetActive(false);      // 游戏开始隐藏
        if (eKeyPrompt != null) eKeyPrompt.enabled = false;
        if (dialogTMPText != null) dialogTMPText.text = ""; // 启动时清空
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (eKeyPrompt != null) eKeyPrompt.enabled = false;

            if (!dialogPanel.activeSelf)
            {
                dialogPanel.SetActive(true);
                typingCoroutine = StartCoroutine(TypeText(message));
            }
            else
            {
                if (typingCoroutine != null)
                {
                    StopCoroutine(typingCoroutine);
                    dialogTMPText.text = message;
                    typingCoroutine = null;
                    StartMiniGame();
                }
            }
        }
    }

    IEnumerator TypeText(string text)
    {
        dialogTMPText.text = "";
        foreach (char c in text)
        {
            dialogTMPText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
        typingCoroutine = null;
        StartMiniGame();
    }

    void StartMiniGame()
    {
        // 关闭对话框并清空文字
        dialogPanel.SetActive(false);
        if (dialogTMPText != null) dialogTMPText.text = "";

        // 切换场景
        if (mainSceneRoot != null) mainSceneRoot.SetActive(false);
        miniGameRoot.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
            if (eKeyPrompt != null) eKeyPrompt.enabled = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
            dialogPanel.SetActive(false);

            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
                typingCoroutine = null;
            }

            if (dialogTMPText != null) dialogTMPText.text = ""; // 离开时清空文字
            if (eKeyPrompt != null) eKeyPrompt.enabled = false;
        }
    }

    /// <summary>
    /// 小游戏结束回到主场景后调用，重置E键提示
    /// </summary>
    public void ResetTrigger()
    {
        dialogPanel.SetActive(false);
        if (dialogTMPText != null) dialogTMPText.text = ""; // 回到主场景时清空文字
        if (eKeyPrompt != null) eKeyPrompt.enabled = true;
        playerInRange = true;    // 让玩家继续可以按E
    }
}
