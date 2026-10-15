using UnityEngine;

public sealed class WeaponVisual : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Transform firePoint;

    private Vector3 firePointBaseLocalPosition;

    private void Awake()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        if (firePoint != null)
        {
            firePointBaseLocalPosition = firePoint.localPosition;
        }
    }

    public void SetFacingLeft(bool facingLeft)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.flipY = facingLeft;
        }

        if (firePoint != null)
        {
            firePoint.localPosition = new Vector3(
                firePointBaseLocalPosition.x,
                facingLeft ? -firePointBaseLocalPosition.y : firePointBaseLocalPosition.y,
                firePointBaseLocalPosition.z);
        }
    }
}