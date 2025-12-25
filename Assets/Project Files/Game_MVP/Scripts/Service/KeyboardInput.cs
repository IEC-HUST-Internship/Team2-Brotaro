using UnityEngine;
using UnityEngine.InputSystem;

public static class KeyboardInput
{
    public static Vector2 GetArrows()
    {
        if (Keyboard.current == null) return Vector2.zero;
        
        var vec = Vector2.zero;
        if (Keyboard.current.aKey.isPressed) vec.x -= 1;
        if (Keyboard.current.dKey.isPressed) vec.x += 1;
        if (Keyboard.current.wKey.isPressed) vec.y += 1;
        if (Keyboard.current.sKey.isPressed) vec.y -= 1;
        return vec.normalized;
    }

    public static bool GetSpacebar()
    {
        return Keyboard.current != null && Keyboard.current.spaceKey.isPressed;
    }
}