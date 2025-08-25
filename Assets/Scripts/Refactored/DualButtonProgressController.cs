using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class DualButtonProgressController : MonoBehaviour
{
    public enum ProgressTargetType { Image, Slider }

    [Header("Buttons")]
    public Button buttonHold;     // 长按按钮
    public Button buttonRapid;    // 连点按钮

    [Header("Progress Target 1 (Hold)")]
    public ProgressTargetType target1Type = ProgressTargetType.Image;
    public Image progressImage1;
    public Slider progressSlider1;

    [Header("Progress Target 2 (Rapid)")]
    public ProgressTargetType target2Type = ProgressTargetType.Slider;
    public Image progressImage2;
    public Slider progressSlider2;

    [Header("Countdown")]
    public TextMeshProUGUI countdownTMP;
    public float countdownTime = 30f;

    [Header("Tuning")]
    public float holdDuration = 3f;           // 长按达到满值所需时长
    public float rapidDecayPerSecond = 0.5f;  // 连点衰减速度
    public float rapidClickIncrease = 0.1f;   // 每次点击提升

    [Header("End Strategy (optional)")]
    public EndStrategyBase endStrategy;       // 可选结束策略（加载场景/切Root等）

    private float _countdown;
    private float _p1; // 0..1
    private float _p2; // 0..1
    private bool _holdPressed;
    private bool _ended;

    void OnEnable()
    {
        Initialize();
        WireButtons();
    }

    void Update()
    {
        if (_ended) return;
        TickCountdown();
        TickHold();
        TickRapid();
        FlushProgress();
        CheckWin();
    }

    public void Initialize()
    {
        _countdown = countdownTime;
        _p1 = 0f;
        _p2 = 0f;
        _holdPressed = false;
        _ended = false;
        SetProgress1(0f);
        SetProgress2(0f);
        UpdateCountdownLabel();
    }

    private void WireButtons()
    {
        if (buttonHold != null)
        {
            var et = buttonHold.gameObject.AddComponent<EventTrigger>();
            var down = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
            down.callback.AddListener(_ => _holdPressed = true);
            var up = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
            up.callback.AddListener(_ => { _holdPressed = false; _p1 = 0f; });
            var exit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
            exit.callback.AddListener(_ => { _holdPressed = false; _p1 = 0f; });
            et.triggers.Add(down);
            et.triggers.Add(up);
            et.triggers.Add(exit);
        }
        if (buttonRapid != null)
        {
            buttonRapid.onClick.RemoveAllListeners();
            buttonRapid.onClick.AddListener(() => { if (!_ended) _p2 = Mathf.Min(_p2 + rapidClickIncrease, 0.99f); });
        }
    }

    private void TickCountdown()
    {
        _countdown -= Time.deltaTime;
        UpdateCountdownLabel();
        if (_countdown <= 0f)
        {
            End("TimeUp");
        }
    }

    private void TickHold()
    {
        if (_holdPressed)
        {
            _p1 += Time.deltaTime / Mathf.Max(0.0001f, holdDuration);
            _p1 = Mathf.Clamp01(_p1);
        }
    }

    private void TickRapid()
    {
        _p2 -= Mathf.Max(0f, rapidDecayPerSecond) * Time.deltaTime;
        if (_p2 < 0f) _p2 = 0f;
    }

    private void FlushProgress()
    {
        SetProgress1(_p1);
        SetProgress2(_p2);
    }

    private void CheckWin()
    {
        if (_p1 >= 1f)
        {
            End("HoldCompleted");
        }
    }

    private void End(string reason)
    {
        if (_ended) return;
        _ended = true;
        if (endStrategy != null)
        {
            endStrategy.OnMiniGameEnd(reason, this);
        }
    }

    private void UpdateCountdownLabel()
    {
        if (countdownTMP != null)
        {
            countdownTMP.text = Mathf.CeilToInt(Mathf.Max(0f, _countdown)).ToString();
        }
    }

    private void SetProgress1(float value)
    {
        switch (target1Type)
        {
            case ProgressTargetType.Image:
                if (progressImage1 != null)
                {
                    progressImage1.type = Image.Type.Filled;
                    progressImage1.fillAmount = value;
                }
                break;
            case ProgressTargetType.Slider:
                if (progressSlider1 != null)
                {
                    progressSlider1.value = value;
                }
                break;
        }
    }

    private void SetProgress2(float value)
    {
        switch (target2Type)
        {
            case ProgressTargetType.Image:
                if (progressImage2 != null)
                {
                    progressImage2.type = Image.Type.Filled;
                    progressImage2.fillAmount = value;
                }
                break;
            case ProgressTargetType.Slider:
                if (progressSlider2 != null)
                {
                    progressSlider2.value = value;
                }
                break;
        }
    }
}

public abstract class EndStrategyBase : MonoBehaviour
{
    public abstract void OnMiniGameEnd(string reason, DualButtonProgressController controller);
}

public class SceneLoadEndStrategy : EndStrategyBase
{
    public string targetSceneName = "SampleScene";
    public float delay = 1f;

    public override void OnMiniGameEnd(string reason, DualButtonProgressController controller)
    {
        controller.StartCoroutine(LoadAfter());
    }

    private IEnumerator LoadAfter()
    {
        yield return new WaitForSeconds(delay);
        if (!string.IsNullOrEmpty(targetSceneName))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(targetSceneName);
        }
    }
}

public class ToggleRootsEndStrategy : EndStrategyBase
{
    public GameObject miniGameRoot;
    public GameObject mainSceneRoot;
    public float delay = 1f;

    public override void OnMiniGameEnd(string reason, DualButtonProgressController controller)
    {
        controller.StartCoroutine(ReturnAfter());
    }

    private IEnumerator ReturnAfter()
    {
        yield return new WaitForSeconds(delay);
        if (miniGameRoot != null) miniGameRoot.SetActive(false);
        if (mainSceneRoot != null) mainSceneRoot.SetActive(true);
    }
} 