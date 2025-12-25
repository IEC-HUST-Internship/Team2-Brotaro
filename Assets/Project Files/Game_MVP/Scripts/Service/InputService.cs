using System;
using UnityEngine;
// 1. Add this namespace to use the New Input System
using UnityEngine.InputSystem; 

namespace SquadShooterMVP
{
    public class InputService : MonoBehaviour
    {
        public Func<Vector2> MovementProvider { get; set; }
        public Vector2 Movement => MovementProvider != null ? MovementProvider() : Vector2.zero;
    private void Awake()
        {
            if (Application.isMobilePlatform)
            {
                var joystick = FindObjectOfType<UIJoystick>();
                
                if (joystick != null)
                {
                    MovementProvider = joystick.GetJoystickInput;
                }
            }
            else
            {
                MovementProvider = KeyboardInput.GetArrows;
                
                var joystick = FindObjectOfType<UIJoystick>();
                if (joystick != null) joystick.gameObject.SetActive(false);
            }
        }
    }
}