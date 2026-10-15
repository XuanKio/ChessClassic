using UnityEngine;

public sealed class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerInputHandler input;
    [SerializeField] private CharacterMovement movement;
    [SerializeField] private CharacterAim characterAim;
    [SerializeField] private WeaponController weaponController;
    [SerializeField] private CharacterHealth health;

    private void Awake()
    {
        if (input == null)
        {
            input = GetComponent<PlayerInputHandler>();
        }
        if (movement == null)
        {
            movement = GetComponent<CharacterMovement>();
        }
        if (characterAim == null)
        {
            characterAim = GetComponent<CharacterAim>();
        }
        if (weaponController == null)
        {
            weaponController = GetComponentInChildren<WeaponController>();
        }
        if (health == null)
        {
            health = GetComponent<CharacterHealth>();
        }
    }

    private void OnEnable()
    {
        if (health == null) return;
        health.OnDied += HandleDeath;

        if (health.IsDead)
        {
            HandleDeath();
        }
    }

    private void OnDisable()
    {
        movement?.ClearMoveInput();
        weaponController?.CancelActions();

        if (health != null)
        {
            health.OnDied -= HandleDeath;
        }
    }

    private void HandleDeath()
    {
        movement?.StopMovement();
        weaponController?.CancelActions();
    }

    private void Update()
    {
        if ((health != null && health.IsDead) || input == null)
        {
            return;
        }

        if (movement != null)
        {
            movement.SetMoveDirection(input.MoveInput);
        }
        if (characterAim != null)
        {
            characterAim.AimAtScreenPosition(input.AimScreenPosition);
        }
        if (weaponController != null)
        {
            if (input.ReloadPressed) weaponController.RequestReload();
            weaponController.SetFireInput(input.IsFiring);
        }
    }
}
