using UnityEngine;

public sealed class WeaponController : MonoBehaviour
{
    [SerializeField] private Transform firePoint;
    [SerializeField] private Sprite bulletSprite;
    [SerializeField, Min(0.01f)] private float fireRate = 0.18f;
    [SerializeField, Min(0f)] private float bulletSpeed = 9f;
    [SerializeField, Min(0.01f)] private float bulletLifetime = 2f;
    [SerializeField, Min(0.01f)] private float bulletScale = 2.5f;
    [SerializeField] private Color bulletColor = Color.yellow;

    private bool fireInput;
private float nextShotTime;

public void SetFireInput(bool isPressed)
    {
        fireInput = isPressed;
    }

    private void Update()
    {
        if (fireInput && Time.time >= nextShotTime)
        {
            Fire();
        }
    }

    private void Fire()
    {
        if (bulletSprite == null)
        {
            return;
        }

        nextShotTime = Time.time + fireRate;
        Vector2 direction = transform.right.normalized;
        Vector3 spawnPosition = firePoint != null
            ? firePoint.position
            : transform.position + (Vector3)(direction * 0.3f);

        GameObject bullet = new GameObject("PlayerBullet");
        SpriteRenderer renderer = bullet.AddComponent<SpriteRenderer>();
        renderer.sprite = bulletSprite;
        renderer.color = bulletColor;
        renderer.sortingLayerName = "Bullet";
        renderer.sortingOrder = 2;

        bullet.transform.position = spawnPosition;
        bullet.transform.rotation = Quaternion.Euler(
            0f, 0f, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
        bullet.transform.localScale = Vector3.one * bulletScale;

        ProjectileMovement projectile = bullet.AddComponent<ProjectileMovement>();
        projectile.Initialize(direction, bulletSpeed, bulletLifetime);
    }
}