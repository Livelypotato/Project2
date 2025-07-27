using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RollingUI : MonoBehaviour
{
    [Header("UI")]
    public Button RB;                    //→按钮
    public Button LB;                    //←按钮   
    //public Sprite[] levelSprites;        //图集(要显示的图片请在Inspector中拖入,顺序需对应关卡序号) 
    //Image[] levelImg;                    //关卡图片 
    Text[] levelText;                    //关卡N文本
    Image[] border;                      //选项边框   
    [ColorUsage(true,false)]
    public Color originColor;            //初始边框颜色
    [ColorUsage(true, false)]
    public Color firstColor;             //最前项边框颜色

    [Header("关卡选项")]
    public GameObject optionPrefab;      //关卡选项预制体
    public Transform OptionGroup;        //选项组父对象
    Transform[] options;                 //选项
    CanvasGroup[] cg;                    //选项的CanvasGroup组件
    [Range(0, 20)]
    public int optionNum;                //选项总数
    float halfNum;                       //选项总数的一半
    //Key：选项    Value：选项位置
    Dictionary<Transform, Vector3> PosDic = new Dictionary<Transform, Vector3>();
    //Key：选项    Value：选项的SiblingIndex
    Dictionary<Transform, int> SiblingDic = new Dictionary<Transform, int>();

    
    Vector3 center = Vector3.zero;        //旋转中心
    [Header("运动参数")]
    public float R = 500f;                //旋转半径
    [Range(1f, 10f)]
    public float speed;                   //旋转速度
    public float yOffset;                 //y轴偏移量
    [Range(0, 1)]
    public float minAlpha;                //最小透明度
    [Range(1, 5)]
    public float firstS;                  //选中项的缩放度
    [Range(0, 1)]
    public float minS;                    //最小缩放度
    [Range(0, 1)]
    public float tempS;                   //旋转过程中的缩放度
    [Range(0, 0.5f)]
    public float smoothSTime;             //缩放平滑时间

    Coroutine currentPIE;                 //当前移动协程
    Coroutine[] SIE2;                     //所有缩放协程

    int first;                            //最前项序号                            

    private void Awake()
    {
        //生成关卡选项
        for (int i = 0; i < optionNum; i++)
        {
            GameObject go = GameObject.Instantiate(optionPrefab, Vector3.zero, Quaternion.identity, OptionGroup);
            go.name = i.ToString();
        }

        halfNum = optionNum / 2;     
        options = new Transform[optionNum];
        cg = new CanvasGroup[optionNum];
        border = new Image[optionNum];
        SIE2 = new Coroutine[optionNum];
        //levelImg = new Image[optionNum];
        levelText = OptionGroup.GetComponentsInChildren<Text>();
        for (int i = 0; i < optionNum; i++)
        {
            options[i] = OptionGroup.GetChild(i);
            cg[i] = options[i].GetComponent<CanvasGroup>();
            //设置关卡序号
            levelText[i].text = "关卡 " + (levelText[i].transform.parent.parent.parent.GetSiblingIndex()+1);
            //获取边框并设置颜色
            border[i] = options[i].GetChild(1).GetComponent<Image>();
            SetBorderColor(i, originColor);
            //levelImg[i] = options[i].GetChild(0).GetChild(0).GetComponent<Image>();           
            //levelImg[i].sprite = levelSprites[i];

        }

        //初始化位置、SiblingIndex、透明度、缩放        
        InitPos();
        InitSibling();
        SetAlpha();
        SetScale();

        //整体居中
        StartCoroutine(AlignCenter());

        //对最前项进行特殊设置
        first = 0;
        SetFirst();
             
        //添加按钮监听
        RB.onClick.AddListener(ClickRight);
        LB.onClick.AddListener(ClickLeft);       
    }

    IEnumerator AlignCenter()
    {
        //确保缩放完成
        for (int i = 0; i < optionNum; i++)
        {
            if (SIE2[i] != null)
            {
                yield return SIE2[i];
            }
        }

        float a = options[0].GetComponent<RectTransform>().rect.height * options[0].localScale.x/2f;
        //偶数
        if (optionNum % 2 == 0)
        {
            float b = options[(int)halfNum].GetComponent<RectTransform>().rect.height * options[(int)halfNum].localScale.x/2f;
            OptionGroup.localPosition = new Vector3(0, (-halfNum * yOffset + a-b) / 2f, 0);
        }
        //奇数
        else
        {
            int temp = (optionNum - 1) / 2;
            float b = options[temp].GetComponent<RectTransform>().rect.height * options[temp].localScale.x/2f;
            OptionGroup.localPosition = new Vector3(0, (-temp * yOffset + a-b) / 2f, 0);
        }
    }

    //初始化位置
    void InitPos()
    {         
        for (int i = 0; i < optionNum; i++)
        {
            float angle = (360.0f / (float)optionNum) * i * Mathf.Deg2Rad;     
            float x = Mathf.Sin(angle)*R;
            float z = -Mathf.Cos(angle)*R;

            float y = 0;
            if (i != 0)
            {            
                if (i > halfNum)
                {                                                       
                    y = (optionNum-i)*yOffset;
                }
                else
                {
                    y = i * yOffset;
                }
            }
          
            //初始化位置并添加到字典里
            Vector3 temp= options[i].localPosition = new Vector3(x, y, z);
            PosDic.Add(options[i], temp);
        }
    }

    //初始化SiblingIndex
    void InitSibling()
    {
        //设置顺序
        for(int i = 0; i < optionNum; i++)
        {
            //未过半
            if (i<= halfNum)
            {   
                //偶数
                if (optionNum % 2 == 0)
                    options[i].SetSiblingIndex((int)halfNum - i);
                //奇数
                else
                    options[i].SetSiblingIndex((int)((optionNum - 1) / 2) - i);
            }

            //过半
            else
            {
                options[i].SetSiblingIndex(options[optionNum - i].GetSiblingIndex());
            }         
        }

        //添加到字典里
        for(int i = 0; i < optionNum; i++)
        {
            SiblingDic.Add(options[i], options[i].GetSiblingIndex());           
        }
    }

    //根据深度（z）设置透明度
    void SetAlpha()
    {
        //计算z值起点，即最前项的z值，此时透明度最大
        float startz = center.z - R;
        for (int i = 0; i < optionNum; i++)
        {
            cg[i].alpha = 1 - Mathf.Abs(options[i].localPosition.z - startz) / (2 * R) * (1 - minAlpha);
        }
    }


    //根据深度（z）设置缩放度
    void SetScale()
    {
        float startz = center.z - R;
        for (int i = 0; i < optionNum; i++)
        {
            //最前项的缩放另行设置
            if (i == first)
            {
                SIE2[i] = StartCoroutine(SmoothScale(options[i], firstS));
            }
            else
            {
                float val = 1 - Mathf.Abs(options[i].localPosition.z - startz) / (2 * R) * (1 - minS);
                SIE2[i] = StartCoroutine(SmoothScale(options[i], val));
            }
        }
    }

    IEnumerator SmoothScale(Transform tf,float targetS)
    {      
        float temp = 0;
        while (Mathf.Abs(tf.localScale.x - targetS) > 0.001)
        {            
            float s = Mathf.SmoothDamp(tf.localScale.x, targetS, ref temp, smoothSTime);
            tf.localScale = Vector3.one * s;
            yield return null;
        }     
    }

    //重置全部选项缩放
    void ResetScale()
    {
        foreach (Transform tf in options)
        {
            tf.localScale = Vector3.one * tempS;
        }
    }

    //设置边框颜色
    void SetBorderColor(int i,Color c)
    {
        border[i].color = c;
    }

    //对当前选项做些特殊设置
    void SetFirst()
    {
        //设置边框颜色
        SetBorderColor(first, firstColor);
        //启用互动
        cg[first].blocksRaycasts = true;
    } 

    //按下按钮后先重置
    void Re()
    {       
        //旋转过程中禁用互动
        cg[first].blocksRaycasts = false;
        //重置缩放
        ResetScale();
        //重置边框颜色
        SetBorderColor(first,originColor);
    }

    void ClickLeft()
    {       
        StartCoroutine(MoveLeft());
    }

    void ClickRight()
    {       
        StartCoroutine(MoveRight());
    }

    IEnumerator MoveLeft()
    {
        //确保所有移动和缩放完成
        if (currentPIE != null)
        {
            yield return currentPIE;
        }
        for (int i = 0; i < optionNum; i++)
        {
            if (SIE2[i] != null)
            {
                yield return SIE2[i];
            }
        }

        //重置
        Re();

        //存储第0个的信息
        Vector3 p = PosDic[options[0]];
        int s = SiblingDic[options[0]];
        Vector3 targetP;
        for (int i = 0; i < optionNum; i++)
        {
            if (i == optionNum - 1)
            {
                targetP = p;
                SiblingDic[options[i]] = s;
            }
            else
            {
                targetP = options[i + 1].localPosition;
                SiblingDic[options[i]] = SiblingDic[options[i + 1]];
            }
            //设置SiblingIndex，并开启移动协程
            options[i].SetSiblingIndex(SiblingDic[options[i]]);
            currentPIE = StartCoroutine(MoveToTarget(options[i], targetP));
        }


        //设置最前项
        if (first == 0)
        {
            first = optionNum - 1;
        }
        else
        {
            first--;
        }
        SetFirst();

        //确保移动完成，设置缩放和透明度
        if (currentPIE != null)
        {
            yield return currentPIE;
        }        
        SetAlpha();
        SetScale();            
    }
    
    IEnumerator MoveRight()
    {
        //确保所有移动和缩放完成
        if (currentPIE != null)
        {
            yield return currentPIE;
        }
        for (int i = 0; i < optionNum; i++)
        {
            if (SIE2[i] != null)
            {
                yield return SIE2[i];
            }
        }

        //重置
        Re();

        //存储最后一个的信息
        Vector3 p = PosDic[options[optionNum - 1]];
        int s = SiblingDic[options[optionNum - 1]];
        Vector3 targetP;
        //从最后一个开始循环
        for (int i = optionNum - 1; i >= 0; i--)
        {
            if (i == 0)
            {
                targetP = p;
                SiblingDic[options[i]] = s;
            }
            else
            {
                targetP = options[i - 1].localPosition;
                SiblingDic[options[i]] = SiblingDic[options[i - 1]];
            }

            //设置SiblingIndex，并开启移动协程
            options[i].SetSiblingIndex(SiblingDic[options[i]]);
            currentPIE = StartCoroutine(MoveToTarget(options[i], targetP));
        }


        //设置最前项
        if (first == optionNum - 1)
        {
            first = 0;
        }
        else
        {
            first++;
        }     
        SetFirst();

        //确保移动完成，设置缩放和透明度
        if (currentPIE != null)
        {
            yield return currentPIE;
        }
        SetAlpha();
        SetScale();
      
    }

    IEnumerator MoveToTarget(Transform tf, Vector3 target)
    {
        float tempspeed = (tf.localPosition - target).magnitude*speed;
        while (tf.localPosition!=target)
        {
            tf.localPosition = Vector3.MoveTowards(tf.localPosition, target, tempspeed * Time.deltaTime);
            yield return null;
        }

        //更新字典里的位置Value
        PosDic[tf] = target;
    }


}
