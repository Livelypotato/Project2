using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class EventHandler : MonoBehaviour
{
    public TMP_Text text01;
    [SerializeField] private PlayerInput playerInput;
    public GameObject face1;
    public SpriteRenderer sr;
    private int todayState;
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("ElevatorButton"))
        {
            if (playerInput.actions["Interact"].triggered)
            {
                //电梯按钮事件
                //播放同事的对话
                //切换scene
            }

        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Clear))
        {
            face1.SetActive(false);
            sr.enabled = false;
        }

        if (todayState < 3)
        {

        }
        else if (todayState >= 4)
        {
            
        }
    }
}
