using UnityEngine;
using UnityEngine.InputSystem;

public sealed class PlayerInputHandler : MonoBehaviour
{
    public Vector2 MoveInput { get; private set; }
    public Vector2 AimScreenPosition { get; private set; }
    public bool IsFiring { get; private set; }

    private void Update()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null)
        {
            MoveInput = Vector2.zero;
        }
        else
        {
            MoveInput = new Vector2(
                (keyboard.aKey.isPressed ? -1f : 0f) + (keyboard.dKey.isPressed ? 1f : 0f),
                (keyboard.sKey.isPressed ? -1f : 0f) + (keyboard.wKey.isPressed ? 1f : 0f)
            ).normalized;
        }

        Mouse mouse = Mouse.current;
        AimScreenPosition = mouse != null ? mouse.position.ReadValue() : Vector2.zero;
        IsFiring = mouse != null &&
            (mouse.leftButton.isPressed || mouse.rightButton.isPressed);
    }
}