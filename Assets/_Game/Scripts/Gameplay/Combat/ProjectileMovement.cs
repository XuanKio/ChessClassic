using UnityEngine;

public sealed class ProjectileMovement : MonoBehaviour
{
    private Vector2 direction;
    private float speed;
    private float lifetime;
    private bool initialized;
    [SerializeField] private LayerMask blockingLayers;
    [SerializeField, Min(0f)] private float collisionRadius = 0.025f;

    public void Initialize(Vector2 moveDirection, float moveSpeed, float duration)
    {
        direction = moveDirection.normalized;
        speed = moveSpeed;
        lifetime = duration;
        if (blockingLayers.value == 0)
            blockingLayers = LayerMask.GetMask("WorldTerrain", "WorldObstacle");
        initialized = true;
    }

    private void Update()
    {
        if (!initialized) return;

        float distance = speed * Time.deltaTime;
        Vector2 position = transform.position;
        RaycastHit2D hit = Physics2D.CircleCast(position, collisionRadius, direction, distance, blockingLayers);
        if (hit.collider != null)
        {
            transform.position = hit.centroid;
            initialized = false;
            Destroy(gameObject);
            return;
        }

        transform.position += (Vector3)(direction * distance);
        lifetime -= Time.deltaTime;
        if (lifetime <= 0f)
            Destroy(gameObject);
    }
}
