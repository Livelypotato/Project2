using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class DialogTyper : MonoBehaviour
{
    [Header("UI")]
    public GameObject dialogPanel;
    public TMP_Text dialogTMPText;
    public SpriteRenderer eKeyPrompt;

    [Header("Typing")]
    [TextArea(3, 10)] public string message;
    public float typingSpeed = 0.05f;
    public bool requireEToStart = true; // 是否需要按E开始

    [Header("Events")]
    public UnityEvent OnDialogShown;
    public UnityEvent OnTypingFinished;
    public UnityEvent OnDialogHidden;

    private bool _playerInRange;
    private Coroutine _typing;
    private bool _finished;

    void Awake()
    {
        if (dialogPanel != null) dialogPanel.SetActive(false);
        if (eKeyPrompt != null) eKeyPrompt.enabled = false;
        if (dialogTMPText != null) dialogTMPText.text = string.Empty;
    }

    void Update()
    {
        if (!_playerInRange) return;
        if (requireEToStart && Input.GetKeyDown(KeyCode.E))
        {
            TogglePrompt(false);
            if (dialogPanel != null && !dialogPanel.activeSelf)
            {
                ShowAndType();
            }
            else if (_typing != null)
            {
                StopCoroutine(_typing);
                _typing = null;
                if (dialogTMPText != null) dialogTMPText.text = message;
                _finished = true;
                OnTypingFinished?.Invoke();
            }
        }
    }

    public void SetPlayerInRange(bool inRange)
    {
        _playerInRange = inRange;
        TogglePrompt(inRange);
        if (!inRange)
        {
            HideDialog();
        }
    }

    public void ShowAndType()
    {
        if (dialogPanel != null) dialogPanel.SetActive(true);
        OnDialogShown?.Invoke();
        _finished = false;
        if (_typing != null) StopCoroutine(_typing);
        _typing = StartCoroutine(TypeText(message));
    }

    public void HideDialog()
    {
        if (dialogPanel != null) dialogPanel.SetActive(false);
        if (_typing != null)
        {
            StopCoroutine(_typing);
            _typing = null;
        }
        if (dialogTMPText != null) dialogTMPText.text = string.Empty;
        TogglePrompt(false);
        OnDialogHidden?.Invoke();
    }

    private IEnumerator TypeText(string text)
    {
        if (dialogTMPText != null) dialogTMPText.text = string.Empty;
        foreach (char c in text)
        {
            if (dialogTMPText != null) dialogTMPText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
        _typing = null;
        _finished = true;
        OnTypingFinished?.Invoke();
    }

    private void TogglePrompt(bool show)
    {
        if (eKeyPrompt != null) eKeyPrompt.enabled = show;
    }
} 