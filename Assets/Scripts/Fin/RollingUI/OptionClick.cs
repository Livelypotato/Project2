using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;//ʹ��IPointerClickHandler�ӿ��轫������
using UnityEngine.SceneManagement;//ʹ��SceneManager�轫������

public class OptionClick : MonoBehaviour, IPointerClickHandler
{

    public void OnPointerClick(PointerEventData eventData)
    {
        
        Debug.Log("����" + GetComponentInChildren<Text>().text);
        
        //��ת����
        //SceneManager.LoadScene(Index);
        //SceneManager.LoadScene("SceneName");
    }

}



