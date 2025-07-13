using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 3f;

    private Vector2 movement;
    private Animator animator;
    private Rigidbody2D rb;

    // 用于记录上一次朝向
    private Vector2 lastDirection = Vector2.down;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        movement = Vector2.zero;

        // 使用 WASD 控制移动方向
        if (Input.GetKey(KeyCode.W))
        {
            movement.y = 1;
            lastDirection = Vector2.up;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            movement.y = -1;
            lastDirection = Vector2.down;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            movement.x = -1;
            lastDirection = Vector2.left;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            movement.x = 1;
            lastDirection = Vector2.right;
        }

        // 移动时更新动画参数
        if (movement != Vector2.zero)
        {
            animator.SetFloat("Horizontal", movement.x);
            animator.SetFloat("Vertical", movement.y);
            animator.SetBool("IsMoving", true);
        }
        else
        {
            // 停止时保留最后方向
            animator.SetFloat("Horizontal", lastDirection.x);
            animator.SetFloat("Vertical", lastDirection.y);
            animator.SetBool("IsMoving", false);
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);
    }
}
