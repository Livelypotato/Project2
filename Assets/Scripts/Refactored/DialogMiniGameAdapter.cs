using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class DialogMiniGameAdapter : MonoBehaviour
{
    [Header("References")]
    public DialogTyper dialogTyper;
    public GameObject mainSceneRoot;
    public GameObject miniGameRoot;

    [Header("Flow")]
    public float switchDelay = 0.2f;
    public UnityEvent OnMiniGameStarted;

    void OnEnable()
    {
        if (dialogTyper != null)
            dialogTyper.OnTypingFinished.AddListener(StartMiniGame);
    }

    void OnDisable()
    {
        if (dialogTyper != null)
            dialogTyper.OnTypingFinished.RemoveListener(StartMiniGame);
    }

    private void StartMiniGame()
    {
        StartCoroutine(SwitchAfterDelay());
    }

    private IEnumerator SwitchAfterDelay()
    {
        yield return new WaitForSeconds(switchDelay);
        if (dialogTyper != null) dialogTyper.HideDialog();
        if (mainSceneRoot != null) mainSceneRoot.SetActive(false);
        if (miniGameRoot != null) miniGameRoot.SetActive(true);
        OnMiniGameStarted?.Invoke();
    }

    // 可供外部调用：小游戏结束回主场景
    public void ReturnToMain()
    {
        if (miniGameRoot != null) miniGameRoot.SetActive(false);
        if (mainSceneRoot != null) mainSceneRoot.SetActive(true);
        // 重新允许交互提示
        if (dialogTyper != null)
        {
            dialogTyper.SetPlayerInRange(true);
        }
    }
} 