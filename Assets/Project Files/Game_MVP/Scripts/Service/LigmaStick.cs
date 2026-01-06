using UnityEngine;
using UnityEngine.EventSystems;

public class LigmaStick : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("References")]
    [SerializeField] private RectTransform container; 
    [SerializeField] private RectTransform background; 
    [SerializeField] private RectTransform handle;    

    [Header("Settings")]
    [SerializeField] private float handleRange = 100f;
    [SerializeField] private float deadZone = 0.1f;
    [SerializeField] private bool snapToFinger = true; 
    [SerializeField] private bool hideOnRelease = false; 

    private Vector2 input = Vector2.zero;
    private Vector2 initialPos;
    private CanvasGroup canvasGroup;

    private void Start()
    {
        initialPos = background.anchoredPosition;
        
        canvasGroup = background.GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = background.gameObject.AddComponent<CanvasGroup>();
        
        if (hideOnRelease) canvasGroup.alpha = 0; 
    }

    public Vector2 GetJoystickInput()
    {
        return input;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (snapToFinger)
        {
            Vector2 localPoint;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                container, 
                eventData.position, 
                eventData.pressEventCamera, 
                out localPoint))
            {
                background.anchoredPosition = localPoint;
            }
        }
        
        if (hideOnRelease) canvasGroup.alpha = 1; 
        
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            background, 
            eventData.position, 
            eventData.pressEventCamera, 
            out localPoint))
        {
            Vector2 normalizedPoint = localPoint / (background.sizeDelta / 2);

            if (normalizedPoint.magnitude > 1)
                normalizedPoint = normalizedPoint.normalized;

            input = (normalizedPoint.magnitude < deadZone) ? Vector2.zero : normalizedPoint;

            // Di chuyển Handle
            handle.anchoredPosition = input * (background.sizeDelta.x / 2) * (handleRange / 100f);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        input = Vector2.zero;
        handle.anchoredPosition = Vector2.zero;
        
        background.anchoredPosition = initialPos;
        
        if (hideOnRelease) canvasGroup.alpha = 0;
    }
}