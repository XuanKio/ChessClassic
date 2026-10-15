using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-50)]
public sealed class PlayerInputHandler : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputActions;
    private InputActionAsset runtimeActions;
    private InputActionMap gameplay;
    private InputAction move;
    private InputAction aim;
    private InputAction fire;
    private InputAction reload;

    public Vector2 MoveInput { get; private set; }
    public Vector2 AimScreenPosition { get; private set; }
    public bool IsFiring { get; private set; }
    public bool ReloadPressed { get; private set; }

    private void Awake()
    {
        if (inputActions == null)
        {
            Debug.LogError("PlayerInputHandler requires PlayerControls input actions.", this);
            enabled = false;
            return;
        }

        runtimeActions = Instantiate(inputActions);
        gameplay = runtimeActions.FindActionMap("Gameplay", true);
        move = gameplay.FindAction("Move", true);
        aim = gameplay.FindAction("Aim", true);
        fire = gameplay.FindAction("Fire", true);
        reload = gameplay.FindAction("Reload", true);
    }

    private void OnEnable() => gameplay?.Enable();

    private void OnDisable()
    {
        gameplay?.Disable();
        MoveInput = Vector2.zero;
        IsFiring = false;
        ReloadPressed = false;
    }

    private void OnDestroy()
    {
        if (runtimeActions != null) Destroy(runtimeActions);
    }

    private void Update()
    {
        MoveInput = Vector2.ClampMagnitude(move.ReadValue<Vector2>(), 1f);
        AimScreenPosition = aim.ReadValue<Vector2>();
        IsFiring = fire.IsPressed();
        ReloadPressed = reload.WasPressedThisFrame();
    }
}
