using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;   // ✅ 支持TextMeshPro

/// <summary>
/// 双按钮倒计时游戏控制器
/// 管理两个具有不同交互机制的按钮和对应的进度条系统
/// </summary>
public class ClickButton : MonoBehaviour
{
    [Header("按钮设置")]
    public Button button1;     // 长按按钮
    public Button button2;     // 连点按钮

    [Header("进度条设置")]
    public Image progressBar1;
    public Image progressBar2;

    [Header("倒计时设置")]
    public TMP_Text countdownText;         // ✅ TextMeshProUGUI
    public float countdownTime = 30f;

    [Header("长按设置")]
    public float longPressDuration = 3f;

    [Header("连续点击设置")]
    public float rapidClickDecayRate = 0.5f;
    public float clickIncrease = 0.1f;

    [Header("场景设置")]
    public string targetSceneName = "SampleScene";

    private float currentCountdown;
    private float button1Progress = 0f;
    private float button2Progress = 0f;
    private bool isButton1Pressing = false;
    private bool gameEnded = false;

    void Start()
    {
        InitializeGame();
        SetupButtonListeners();
    }

    void Update()
    {
        if (gameEnded) return;

        UpdateCountdown();
        UpdateButton1Progress();
        UpdateButton2Progress();
        UpdateProgressBars();
        CheckWinConditions();
    }

    void InitializeGame()
    {
        currentCountdown = countdownTime;
        button1Progress = 0f;
        button2Progress = 0f;
        isButton1Pressing = false;
        gameEnded = false;

        if (progressBar1 != null)
        {
            progressBar1.fillAmount = 0f;
            progressBar1.type = Image.Type.Filled;
        }
        if (progressBar2 != null)
        {
            progressBar2.fillAmount = 0f;
            progressBar2.type = Image.Type.Filled;
        }
        UpdateCountdownDisplay();
    }

    void SetupButtonListeners()
    {
        if (button1 != null)
        {
            var eventTrigger1 = button1.gameObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();

            var pointerDownEntry1 = new UnityEngine.EventSystems.EventTrigger.Entry();
            pointerDownEntry1.eventID = UnityEngine.EventSystems.EventTriggerType.PointerDown;
            pointerDownEntry1.callback.AddListener((data) => { StartButton1Press(); });
            eventTrigger1.triggers.Add(pointerDownEntry1);

            var pointerUpEntry1 = new UnityEngine.EventSystems.EventTrigger.Entry();
            pointerUpEntry1.eventID = UnityEngine.EventSystems.EventTriggerType.PointerUp;
            pointerUpEntry1.callback.AddListener((data) => { StopButton1Press(); });
            eventTrigger1.triggers.Add(pointerUpEntry1);

            var pointerExitEntry1 = new UnityEngine.EventSystems.EventTrigger.Entry();
            pointerExitEntry1.eventID = UnityEngine.EventSystems.EventTriggerType.PointerExit;
            pointerExitEntry1.callback.AddListener((data) => { StopButton1Press(); });
            eventTrigger1.triggers.Add(pointerExitEntry1);
        }

        if (button2 != null)
        {
            button2.onClick.AddListener(OnButton2Click);
        }
    }

    void StartButton1Press()
    {
        isButton1Pressing = true;
    }

    void StopButton1Press()
    {
        isButton1Pressing = false;
        button1Progress = 0f;
    }

    void OnButton2Click()
    {
        if (gameEnded) return;
        button2Progress = Mathf.Min(button2Progress + clickIncrease, 0.99f);
    }

    void UpdateCountdown()
    {
        currentCountdown -= Time.deltaTime;
        UpdateCountdownDisplay();
        if (currentCountdown <= 0f)
        {
            EndGame("倒计时结束");
        }
    }

    void UpdateButton1Progress()
    {
        if (isButton1Pressing && !gameEnded)
        {
            button1Progress += Time.deltaTime / longPressDuration;
            button1Progress = Mathf.Clamp01(button1Progress);
        }
    }

    void UpdateButton2Progress()
    {
        if (!gameEnded)
        {
            button2Progress -= rapidClickDecayRate * Time.deltaTime;
            button2Progress = Mathf.Max(button2Progress, 0f);
        }
    }

    void UpdateProgressBars()
    {
        if (progressBar1 != null)
        {
            progressBar1.fillAmount = button1Progress;
        }
        if (progressBar2 != null)
        {
            progressBar2.fillAmount = button2Progress;
        }
    }

    void UpdateCountdownDisplay()
    {
        if (countdownText != null)
        {
            int seconds = Mathf.CeilToInt(currentCountdown);
            countdownText.text = $"倒计时: {seconds}秒";
        }
    }

    void CheckWinConditions()
    {
        if (button1Progress >= 1f)
        {
            EndGame("按钮1长按成功完成");
        }
    }

    void EndGame(string reason)
    {
        if (gameEnded) return;
        gameEnded = true;

        Debug.Log($"游戏结束原因: {reason}");
        StartCoroutine(LoadSceneAfterDelay(1f));
    }

    IEnumerator LoadSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (Application.CanStreamedLevelBeLoaded(targetSceneName))
        {
            SceneManager.LoadScene(targetSceneName);
        }
        else
        {
            Debug.LogWarning($"场景 '{targetSceneName}' 不存在，重新加载当前场景");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public void ResetGame()
    {
        InitializeGame();
    }
}

/*
使用步骤：
① 挂在空GameObject上；
② 把Button、Image、TextMeshProUGUI拖进对应槽位；
③ 调好参数就可以开始玩啦！
*/
