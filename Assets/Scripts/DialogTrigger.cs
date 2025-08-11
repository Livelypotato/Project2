using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DialogTrigger : MonoBehaviour
{
    public GameObject dialogPanel;   // 对话框UI整体
    public Text dialogText;          // 对话框中显示文字的Text组件
    [TextArea(3,10)]
    public string message;           // 要显示的文字内容
    public float typingSpeed = 0.05f; // 文字逐字显示速度

    private bool playerInRange = false;
    private Coroutine typingCoroutine;

    void Start()
    {
        dialogPanel.SetActive(false); // 一开始隐藏对话框
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            // 按E键时，如果对话框没显示，就显示并开始打字效果
            if (!dialogPanel.activeSelf)
            {
                dialogPanel.SetActive(true);
                typingCoroutine = StartCoroutine(TypeText(message));
            }
            else
            {
                // 如果对话框已显示，按E键可以立即显示全部文字（跳过打字）
                if (typingCoroutine != null)
                {
                    StopCoroutine(typingCoroutine);
                    dialogText.text = message;
                }
            }
        }
    }

    IEnumerator TypeText(string text)
    {
        dialogText.text = "";
        foreach (char c in text)
        {
            dialogText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
        typingCoroutine = null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
            // 你也可以这里自动弹出对话框，或者只靠按E弹出
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
        }
    }
}