using UnityEngine;
using UnityEngine.InputSystem; 

public class InputService : MonoBehaviour
{
    [SerializeField] private LigmaStick _joystick; 

    public Vector2 JoystickInput
    {
        get
        {
            if (_joystick != null)
            {
                Vector2 joyInput = _joystick.GetJoystickInput();
                if (joyInput != Vector2.zero) return joyInput;
            }

            Vector2 keyInput = Vector2.zero;
            
            if (Keyboard.current != null)
            {
                if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) keyInput.x -= 1;
                if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) keyInput.x += 1;
                if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) keyInput.y += 1;
                if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) keyInput.y -= 1;
            }

            return keyInput.normalized;
        }
    }
}