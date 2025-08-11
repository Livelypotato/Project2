using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HoldToFillProgress : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Image progressBar;
    [SerializeField] private float fillSpeed = 0.5f;

    [SerializeField] private ButtonsUIManager uiManager;

    private float progress = 0f;
    private bool isHolding = false;

    void Update()
    {
        if (isHolding && progress < 1f)
        {
            progress += fillSpeed * Time.deltaTime;
            if (progress > 1f) progress = 1f;
            progressBar.fillAmount = progress;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isHolding = true;
        if (uiManager != null)
            uiManager.ShowPressedUI();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isHolding = false;
        if (uiManager != null)
            uiManager.ShowNormalUI();
    }
}