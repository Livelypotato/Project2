using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private PanelAnim PanelAnim; // 引用PanelAnim脚本
    public enum GameState
    {
        MainMenu,
        Playing,
        Paused,
        GameOver
    }
    public GameState currentState;

    public List<string> moods;  // List to hold mood strings

    private List<string> lastMoods = new List<string>(); // 用于检测变动

    void Start()
    {
        moods.Add("Happy");
        moods.Add("Sad");
        lastMoods = new List<string>(moods); // 初始化快照
    }

    void Update()
    {
        // 检查moods是否有变动
        if (moods.Count != lastMoods.Count)
        {
            // 找到新增的内容
            foreach (var mood in moods)
            {
                if (!lastMoods.Contains(mood))
                {
                    PanelAnim.ShowAchievement(mood); // 调用动画
                }
            }
            lastMoods = new List<string>(moods); // 更新快照
        }
        else
        {
            // 检查内容变动
            for (int i = 0; i < moods.Count; i++)
            {
                if (moods[i] != lastMoods[i])
                {
                    PanelAnim.ShowAchievement(moods[i]);
                    lastMoods = new List<string>(moods);
                    break;
                }
            }
        }
    }
}
