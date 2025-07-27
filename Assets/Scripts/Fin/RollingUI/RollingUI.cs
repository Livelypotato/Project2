using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RollingUI : MonoBehaviour
{
    [Header("UI")]
    public Button RB;                    //����ť
    public Button LB;                    //����ť   
    //public Sprite[] levelSprites;        //ͼ��(Ҫ��ʾ��ͼƬ����Inspector������,˳�����Ӧ�ؿ����) 
    //Image[] levelImg;                    //�ؿ�ͼƬ 
    Text[] levelText;                    //�ؿ�N�ı�
    Image[] border;                      //ѡ��߿�   
    [ColorUsage(true,false)]
    public Color originColor;            //��ʼ�߿���ɫ
    [ColorUsage(true, false)]
    public Color firstColor;             //��ǰ��߿���ɫ

    [Header("�ؿ�ѡ��")]
    public GameObject optionPrefab;      //�ؿ�ѡ��Ԥ����
    public Transform OptionGroup;        //ѡ���鸸����
    Transform[] options;                 //ѡ��
    CanvasGroup[] cg;                    //ѡ���CanvasGroup���
    [Range(0, 20)]
    public int optionNum;                //ѡ������
    float halfNum;                       //ѡ��������һ��
    //Key��ѡ��    Value��ѡ��λ��
    Dictionary<Transform, Vector3> PosDic = new Dictionary<Transform, Vector3>();
    //Key��ѡ��    Value��ѡ���SiblingIndex
    Dictionary<Transform, int> SiblingDic = new Dictionary<Transform, int>();

    
    Vector3 center = Vector3.zero;        //��ת����
    [Header("�˶�����")]
    public float R = 500f;                //��ת�뾶
    [Range(1f, 10f)]
    public float speed;                   //��ת�ٶ�
    public float yOffset;                 //y��ƫ����
    [Range(0, 1)]
    public float minAlpha;                //��С͸����
    [Range(1, 5)]
    public float firstS;                  //ѡ��������Ŷ�
    [Range(0, 1)]
    public float minS;                    //��С���Ŷ�
    [Range(0, 1)]
    public float tempS;                   //��ת�����е����Ŷ�
    [Range(0, 0.5f)]
    public float smoothSTime;             //����ƽ��ʱ��

    Coroutine currentPIE;                 //��ǰ�ƶ�Э��
    Coroutine[] SIE2;                     //��������Э��

    int first;                            //��ǰ�����                            

    private void Awake()
    {
        //���ɹؿ�ѡ��
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
            //���ùؿ����
            levelText[i].text = "�ؿ� " + (levelText[i].transform.parent.parent.parent.GetSiblingIndex()+1);
            //��ȡ�߿�������ɫ
            border[i] = options[i].GetChild(1).GetComponent<Image>();
            SetBorderColor(i, originColor);
            //levelImg[i] = options[i].GetChild(0).GetChild(0).GetComponent<Image>();           
            //levelImg[i].sprite = levelSprites[i];

        }

        //��ʼ��λ�á�SiblingIndex��͸���ȡ�����        
        InitPos();
        InitSibling();
        SetAlpha();
        SetScale();

        //�������
        StartCoroutine(AlignCenter());

        //����ǰ�������������
        first = 0;
        SetFirst();
             
        //��Ӱ�ť����
        RB.onClick.AddListener(ClickRight);
        LB.onClick.AddListener(ClickLeft);       
    }

    IEnumerator AlignCenter()
    {
        //ȷ���������
        for (int i = 0; i < optionNum; i++)
        {
            if (SIE2[i] != null)
            {
                yield return SIE2[i];
            }
        }

        float a = options[0].GetComponent<RectTransform>().rect.height * options[0].localScale.x/2f;
        //ż��
        if (optionNum % 2 == 0)
        {
            float b = options[(int)halfNum].GetComponent<RectTransform>().rect.height * options[(int)halfNum].localScale.x/2f;
            OptionGroup.localPosition = new Vector3(0, (-halfNum * yOffset + a-b) / 2f, 0);
        }
        //����
        else
        {
            int temp = (optionNum - 1) / 2;
            float b = options[temp].GetComponent<RectTransform>().rect.height * options[temp].localScale.x/2f;
            OptionGroup.localPosition = new Vector3(0, (-temp * yOffset + a-b) / 2f, 0);
        }
    }

    //��ʼ��λ��
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
          
            //��ʼ��λ�ò���ӵ��ֵ���
            Vector3 temp= options[i].localPosition = new Vector3(x, y, z);
            PosDic.Add(options[i], temp);
        }
    }

    //��ʼ��SiblingIndex
    void InitSibling()
    {
        //����˳��
        for(int i = 0; i < optionNum; i++)
        {
            //δ����
            if (i<= halfNum)
            {   
                //ż��
                if (optionNum % 2 == 0)
                    options[i].SetSiblingIndex((int)halfNum - i);
                //����
                else
                    options[i].SetSiblingIndex((int)((optionNum - 1) / 2) - i);
            }

            //����
            else
            {
                options[i].SetSiblingIndex(options[optionNum - i].GetSiblingIndex());
            }         
        }

        //��ӵ��ֵ���
        for(int i = 0; i < optionNum; i++)
        {
            SiblingDic.Add(options[i], options[i].GetSiblingIndex());           
        }
    }

    //������ȣ�z������͸����
    void SetAlpha()
    {
        //����zֵ��㣬����ǰ���zֵ����ʱ͸�������
        float startz = center.z - R;
        for (int i = 0; i < optionNum; i++)
        {
            cg[i].alpha = 1 - Mathf.Abs(options[i].localPosition.z - startz) / (2 * R) * (1 - minAlpha);
        }
    }


    //������ȣ�z���������Ŷ�
    void SetScale()
    {
        float startz = center.z - R;
        for (int i = 0; i < optionNum; i++)
        {
            //��ǰ���������������
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

    //����ȫ��ѡ������
    void ResetScale()
    {
        foreach (Transform tf in options)
        {
            tf.localScale = Vector3.one * tempS;
        }
    }

    //���ñ߿���ɫ
    void SetBorderColor(int i,Color c)
    {
        border[i].color = c;
    }

    //�Ե�ǰѡ����Щ��������
    void SetFirst()
    {
        //���ñ߿���ɫ
        SetBorderColor(first, firstColor);
        //���û���
        cg[first].blocksRaycasts = true;
    } 

    //���°�ť��������
    void Re()
    {       
        //��ת�����н��û���
        cg[first].blocksRaycasts = false;
        //��������
        ResetScale();
        //���ñ߿���ɫ
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
        //ȷ�������ƶ����������
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

        //����
        Re();

        //�洢��0������Ϣ
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
            //����SiblingIndex���������ƶ�Э��
            options[i].SetSiblingIndex(SiblingDic[options[i]]);
            currentPIE = StartCoroutine(MoveToTarget(options[i], targetP));
        }


        //������ǰ��
        if (first == 0)
        {
            first = optionNum - 1;
        }
        else
        {
            first--;
        }
        SetFirst();

        //ȷ���ƶ���ɣ��������ź�͸����
        if (currentPIE != null)
        {
            yield return currentPIE;
        }        
        SetAlpha();
        SetScale();            
    }
    
    IEnumerator MoveRight()
    {
        //ȷ�������ƶ����������
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

        //����
        Re();

        //�洢���һ������Ϣ
        Vector3 p = PosDic[options[optionNum - 1]];
        int s = SiblingDic[options[optionNum - 1]];
        Vector3 targetP;
        //�����һ����ʼѭ��
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

            //����SiblingIndex���������ƶ�Э��
            options[i].SetSiblingIndex(SiblingDic[options[i]]);
            currentPIE = StartCoroutine(MoveToTarget(options[i], targetP));
        }


        //������ǰ��
        if (first == optionNum - 1)
        {
            first = 0;
        }
        else
        {
            first++;
        }     
        SetFirst();

        //ȷ���ƶ���ɣ��������ź�͸����
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

        //�����ֵ����λ��Value
        PosDic[tf] = target;
    }


}
