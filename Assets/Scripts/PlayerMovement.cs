using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5f; // 控制移动速度
    private Rigidbody2D _rb; // 2D 物理组件

    private PlayerInput _playerInput; // 玩家输入组件
    private InputAction _moveAction; // 移动的输入动作

    private Animator _animator; // 动画控制器

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>(); // 获取刚体组件
        _playerInput = GetComponent<PlayerInput>(); // 获取输入系统组件
        _moveAction = _playerInput.actions["Move"]; // 获取"Move"动作
        _animator = GetComponent<Animator>(); // 获取动画控制器
    }

    void Update()
    {
        Vector2 move = _moveAction.ReadValue<Vector2>(); // 获取移动方向输入
        _rb.linearVelocity = move * speed; // 设置玩家刚体速度

        //Debug.Log("_rb.linearVelocity:"+_rb.linearVelocity.magnitude);
        //Debug.Log("move:"+move);
        // 更新动画参数，让动画表现出正确方向
        _animator.SetFloat("Horizontal", move.x);
        _animator.SetFloat("Vertical", move.y);
        _animator.SetFloat("Speed", _rb.linearVelocity.magnitude);

        // 如果正在移动，记录最后移动方向（用于站立朝向）
        if (move != Vector2.zero)
        {
            _animator.SetFloat("LastHorizontal", move.x);
            _animator.SetFloat("LastVertical", move.y);
        }
    }
}