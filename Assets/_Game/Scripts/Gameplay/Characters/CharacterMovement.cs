using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public sealed class CharacterMovement : MonoBehaviour
{
    [SerializeField] private CharacterData characterData;
    [SerializeField] private Rigidbody2D body;

    private Vector2 moveInput;

    private void Awake()
    {
        if (body == null)
        {
            body = GetComponent<Rigidbody2D>();
        }
    }

public void SetMoveDirection(Vector2 direction)
    {
        moveInput = Vector2.ClampMagnitude(direction, 1f);
    }

private void FixedUpdate()
    {
        if (body == null || characterData == null)
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