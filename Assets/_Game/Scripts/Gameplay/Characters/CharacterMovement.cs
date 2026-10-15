using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public sealed class CharacterMovement : MonoBehaviour
{
    [SerializeField] private CharacterData characterData;
    [SerializeField] private Rigidbody2D body;

    private Vector2 moveInput;
    private bool movementEnabled = true;

    private void Awake()
    {
        if (body == null)
        {
            body = GetComponent<Rigidbody2D>();
        }
    }

    public void SetMoveDirection(Vector2 direction)
    {
        if (!movementEnabled)
        {
            return;
        }

        moveInput = Vector2.ClampMagnitude(direction, 1f);
    }

    public void StopMovement()
    {
        movementEnabled = false;
        ClearMoveInput();
    }

    // Temporary stop for controller disable; unlike death, this does not lock movement.
    public void ClearMoveInput()
    {
        moveInput = Vector2.zero;

        if (body != null)
        {
            body.linearVelocity = Vector2.zero;
        }
    }

    private void OnDisable()
    {
        ClearMoveInput();
    }

    private void FixedUpdate()
    {
        if (!movementEnabled || body == null || characterData == null)
        {
            return;
        }

        Vector2 targetVelocity = moveInput * characterData.MoveSpeed;
        float speedChange = moveInput.sqrMagnitude > 0f
            ? characterData.Acceleration
            : characterData.Deceleration;

        body.linearVelocity = Vector2.MoveTowards(
            body.linearVelocity,
            targetVelocity,
            speedChange * Time.fixedDeltaTime);
    }
}
