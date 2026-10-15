using UnityEngine;

// This view belongs to the local player's screen-space overlay HUD.
[RequireComponent(typeof(RectTransform))]
public sealed class CrosshairView : MonoBehaviour
{
    [SerializeField] private PlayerInputHandler input;
    private bool previousCursorVisible;

    private void OnEnable()
    {
        previousCursorVisible = Cursor.visible;
        Cursor.visible = false;
    }

    private void LateUpdate()
    {
        if (input != null) transform.position = input.AimScreenPosition;
    }

    private void OnDisable()
    {
        Cursor.visible = previousCursorVisible;
    }
}
