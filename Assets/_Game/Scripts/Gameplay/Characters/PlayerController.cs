using UnityEngine;

public sealed class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerInputHandler input;
    [SerializeField] private CharacterMovement movement;
    [SerializeField] private CharacterAim characterAim;
    [SerializeField] private WeaponController weaponController;

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
    }

    private void Update()
    {
        if (input == null)
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
            weaponController.SetFireInput(input.IsFiring);
        }
    }
}