using UnityEngine;

// Owns projectile creation and the muzzle clearance check. Appearance lives in the prefab.
public sealed class ProjectileSpawner : MonoBehaviour
{
    [SerializeField] private ProjectileMovement projectilePrefab;
    [SerializeField] private LayerMask blockingLayers;

    public bool IsConfigured => projectilePrefab != null;

    public bool TrySpawn(Transform firePoint, Vector2 direction, WeaponData data)
    {
        if (projectilePrefab == null || data == null) return false;

        Vector3 spawnPosition = firePoint != null
            ? firePoint.position
            : transform.position + (Vector3)(direction * 0.3f);

        if (Physics2D.Linecast(transform.root.position, spawnPosition, blockingLayers).collider != null)
            return false;

        Quaternion rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
        ProjectileMovement projectile = Instantiate(projectilePrefab, spawnPosition, rotation);
        projectile.Initialize(direction, data.BulletSpeed, data.BulletLifetime);
        return true;
    }
}
