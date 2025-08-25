using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ClickButtonMiniGame : MonoBehaviour
{
    public GameObject miniGameRoot;      // 小游戏父物体（本物体）
    public GameObject mainSceneRoot;     // 主场景Root

    public Button button1;
    public Button button2;
    
    public Slider progressSlider1;
    public Slider progressSlider2;

    public TextMeshProUGUI countdownTMP;
    public float countdownTime = 30f;

    public float longPressDuration = 3f;
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
        progressSlider1.value = button1Progress; // 更新进度条组件
        progressSlider2.value = button2Progress; // 更新进度条组件
    }

    void UpdateCountdownDisplay()
    {
        if (countdownTMP != null)
        {
            countdownTMP.text = $" {Mathf.CeilToInt(currentCountdown)}";
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

        // 让DialogTriggerMiniGame重新生效
        var trigger = mainSceneRoot.GetComponentInChildren<DialogTriggerMiniGame>();
        if (trigger != null)
        {
            trigger.ResetTrigger();
        }

        InitializeGame();
    }
}
