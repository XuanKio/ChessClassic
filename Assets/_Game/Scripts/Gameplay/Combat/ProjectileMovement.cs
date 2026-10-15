using UnityEngine;

public sealed class ProjectileMovement : MonoBehaviour
{
    private Vector2 direction;
    private float speed;
    private float lifetime;
    private bool initialized;

    public void Initialize(Vector2 moveDirection, float moveSpeed, float duration)
    {
        direction = moveDirection.normalized;
        speed = moveSpeed;
        lifetime = duration;
        initialized = true;
    }

    private void Update()
    {
        if (!initialized)
        {
            return;
        }

        transform.position += (Vector3)(direction * speed * Time.deltaTime);
        lifetime -= Time.deltaTime;

        if (lifetime <= 0f)
        {
            Destroy(gameObject);
        }
    }
}