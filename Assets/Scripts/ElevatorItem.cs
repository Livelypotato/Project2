using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class ElevatorItem : TriggerBase
{
    bool _isAllowMoving = false;
    public PlayerInput _playerInput; // 玩家输入组件
    private InputAction _downAction; // 移动的输入动作
    [SerializeField] GameObject PressHint; // 提示按键的UI

    public UnityEvent OnDownElevator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (_playerInput == null)
        {
            _playerInput = FindObjectOfType<PlayerInput>();
        }

        _downAction = _playerInput.actions["Elevator"]; // 获取"Move"动作
    }

    // Update is called once per frame
    void Update()
    {
    }


    void OnElevatorAction(InputAction.CallbackContext context)
    {
        OnDownElevator?.Invoke();
    }

    public override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
        if (other.CompareTag(TargetTag))
        {
            PressHint.SetActive(true); // 显示提示UI
            _isAllowMoving = true;
            _downAction.performed += OnElevatorAction;
        }
    }


    public override void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(TargetTag))
        {
            PressHint.SetActive(false); // 隐藏提示UI
            _isAllowMoving = false;
            _downAction.performed -= OnElevatorAction;
        }
    }
}