using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class TriggerBase : MonoBehaviour
{
    public string TargetTag;
    

    public UnityEvent OnTriggerEnterEvent;
    public UnityEvent OnTriggerExitEvent;
    public UnityEvent OnTriggerStayEvent;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(TargetTag))
        {
            OnTriggerEnterEvent?.Invoke();
        }
    }

    public virtual void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag(TargetTag))
        {
            OnTriggerStayEvent?.Invoke();
        }
    }


    public virtual void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(TargetTag))
        {
            OnTriggerExitEvent?.Invoke();
        }
        
    }
}
