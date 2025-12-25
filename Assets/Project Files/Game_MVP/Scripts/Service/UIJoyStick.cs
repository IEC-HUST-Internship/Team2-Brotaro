using UnityEngine;
using UnityEngine.EventSystems;

namespace SquadShooterMVP
{
    public class UIJoystick : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        [Header("References")]
        [SerializeField] private RectTransform background;
        [SerializeField] private RectTransform handle;

        [Header("Settings")]
        [SerializeField] private float handleRange = 100f; 
        [SerializeField] private float deadZone = 0.1f;  

        // The processed input vector (-1 to 1)
        private Vector2 input = Vector2.zero;

        // ---------------------------------------------------------
        // Public API (This is what InputService plugs into!)
        // ---------------------------------------------------------
        public Vector2 GetJoystickInput()
        {
            return input;
        }

        // ---------------------------------------------------------
        // UI Event Handlers
        // ---------------------------------------------------------
        
        public void OnPointerDown(PointerEventData eventData)
        {
            // When touched, immediately treat it as a drag start
            OnDrag(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector2 localPoint;

            // 1. Convert Screen Touch -> Local UI Coordinates
            // eventData.pressEventCamera handles both Overlay and Camera render modes automatically
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                background, 
                eventData.position, 
                eventData.pressEventCamera, 
                out localPoint))
            {
                // 2. Normalize the position (0,0 is center, 1,1 is edge)
                // We divide by sizeDelta / 2 because the pivot is in the center
                Vector2 normalizedPoint = localPoint / (background.sizeDelta / 2);

                // 3. Limit the vector to a circle (magnitude of 1)
                // This prevents the player from moving faster diagonally if that's not desired
                if (normalizedPoint.magnitude > 1)
                {
                    normalizedPoint = normalizedPoint.normalized;
                }

                // 4. Apply Deadzone (Optional, good for loose thumbs)
                input = (normalizedPoint.magnitude < deadZone) ? Vector2.zero : normalizedPoint;

                // 5. Move the Visual Handle
                // We multiply by handleRange to keep the visual movement within the background circle
                handle.anchoredPosition = input * (background.sizeDelta.x / 2) * (handleRange / 100f);
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            // Reset everything when finger is lifted
            input = Vector2.zero;
            handle.anchoredPosition = Vector2.zero;
        }
    }
}