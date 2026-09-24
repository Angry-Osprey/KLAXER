using UnityEngine;
using UnityEngine.InputSystem;

// The one place that knows which devices drive the game. Pointer covers mouse, pen and touch.
public static class GameInput
{
    // -1..1 from the arrow keys or A/D.
    public static float MoveAxis
    {
        get
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard == null) return 0f;

            float axis = 0f;
            if (keyboard.leftArrowKey.isPressed || keyboard.aKey.isPressed) axis -= 1f;
            if (keyboard.rightArrowKey.isPressed || keyboard.dKey.isPressed) axis += 1f;
            return axis;
        }
    }

    // True while a mouse button or finger is held down.
    public static bool TryGetHeldPointer(out Vector2 screenPosition)
    {
        Pointer pointer = Pointer.current;
        bool held = pointer != null && pointer.press.isPressed;
        screenPosition = held ? pointer.position.ReadValue() : default;
        return held;
    }

    // Space, or lifting the finger / mouse button, so a drag can line the paddle up first.
    public static bool LaunchTriggered =>
        (Keyboard.current?.spaceKey.wasPressedThisFrame ?? false) || PointerReleased;

    public static bool AnyKeyPressed => Keyboard.current?.anyKey.wasPressedThisFrame ?? false;

    public static bool PointerPressed => Pointer.current?.press.wasPressedThisFrame ?? false;

    public static bool PointerReleased => Pointer.current?.press.wasReleasedThisFrame ?? false;
}
