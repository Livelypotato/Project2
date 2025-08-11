using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ClickToFillProgress : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Image progressBar;
    [SerializeField] private float increment = 0.1f;

    [SerializeField] private ButtonsUIManager uiManager;

    private float progress = 0f;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (progress < 1f)
        {
            progress += increment;
            if (progress > 1f) progress = 1f;
            progressBar.fillAmount = progress;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (uiManager != null)
            uiManager.ShowPressedUI();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (uiManager != null)
            uiManager.ShowNormalUI();
    }
}