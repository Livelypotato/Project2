using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 挂在小游戏整体MiniGameRoot上
/// </summary>
public class ClickButtonMiniGame : MonoBehaviour
{
    [Header("场景切换控制")]
    public GameObject miniGameRoot;      // 小游戏父物体（本物体）
    public GameObject mainSceneRoot;     // 主场景Root

    [Header("按钮设置")]
    public Button button1;      
    public Button button2;

    [Header("进度条设置")]
    public Image progressBar1;
    public Image progressBar2;

    [Header("倒计时设置 (TMP)")]
    public TextMeshProUGUI countdownTMP;     // ← 使用TMP组件
    public float countdownTime = 30f;

    [Header("长按设置")]
    public float longPressDuration = 3f;

    [Header("连续点击设置")]
    public float rapidClickDecayRate = 0.5f;
    public float clickIncrease = 0.1f;

    private float currentCountdown;
    private float button1Progress = 0f;
    private float button2Progress = 0f;
    private bool isButton1Pressing = false;
    private bool gameEnded = false;

    void OnEnable()
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

        progressBar1.fillAmount = 0f;
        progressBar2.fillAmount = 0f;
        UpdateCountdownDisplay();
    }

    void SetupButtonListeners()
    {
        var trigger = button1.gameObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();
        var down = new UnityEngine.EventSystems.EventTrigger.Entry
        {
            eventID = UnityEngine.EventSystems.EventTriggerType.PointerDown
        };
        down.callback.AddListener((_) => { isButton1Pressing = true; });
        var up = new UnityEngine.EventSystems.EventTrigger.Entry
        {
            eventID = UnityEngine.EventSystems.EventTriggerType.PointerUp
        };
        up.callback.AddListener((_) =>
        {
            isButton1Pressing = false;
            button1Progress = 0f;
        });
        trigger.triggers.Add(down);
        trigger.triggers.Add(up);

        button2.onClick.RemoveAllListeners();
        button2.onClick.AddListener(() =>
        {
            if (!gameEnded)
                button2Progress = Mathf.Min(button2Progress + clickIncrease, 0.99f);
        });
    }

    void UpdateCountdown()
    {
        currentCountdown -= Time.deltaTime;
        UpdateCountdownDisplay();
        if (currentCountdown <= 0f)
        {
            EndGame();
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
        progressBar1.fillAmount = button1Progress;
        progressBar2.fillAmount = button2Progress;
    }

    void UpdateCountdownDisplay()
    {
        if (countdownTMP != null)
        {
            countdownTMP.text = $"倒计时: {Mathf.CeilToInt(currentCountdown)}秒";
        }
    }

    void CheckWinConditions()
    {
        if (button1Progress >= 1f)
        {
            EndGame();
        }
    }

    void EndGame()
    {
        if (gameEnded) return;
        gameEnded = true;
        StartCoroutine(ReturnToMain());
    }

    IEnumerator ReturnToMain()
    {
        yield return new WaitForSeconds(1f);
        miniGameRoot.SetActive(false);
        mainSceneRoot.SetActive(true);
        InitializeGame();
    }
}
