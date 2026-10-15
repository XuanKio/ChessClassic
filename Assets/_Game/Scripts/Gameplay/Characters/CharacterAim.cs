using UnityEngine;

public sealed class CharacterAim : MonoBehaviour
{
    [SerializeField] private Transform weaponPivot;
    [SerializeField] private WeaponVisual weaponVisual;
    [SerializeField] private SpriteRenderer characterRenderer;

    private Camera gameCamera;
    private Vector3 weaponHoldOffset;

    private void Awake()
    {
        gameCamera = Camera.main;

        if (characterRenderer == null)
        {
            Transform visual = transform.Find("PlayerVisual");
            if (visual != null)
            {
                characterRenderer = visual.GetComponent<SpriteRenderer>();
            }
        }

        if (weaponPivot != null)
        {
            weaponHoldOffset = weaponPivot.localPosition;
        }
    }

    public void AimAtScreenPosition(Vector2 mouseScreen)
    {
        if (gameCamera == null)
        {
            gameCamera = Camera.main;
        }
        if (gameCamera == null || weaponPivot == null)
        {
            return;
        }

        float cameraDistance = Mathf.Abs(gameCamera.transform.position.z - transform.position.z);
        Vector3 mouseWorld = gameCamera.ScreenToWorldPoint(
            new Vector3(mouseScreen.x, mouseScreen.y, cameraDistance));

        Vector2 direction = (Vector2)(mouseWorld - transform.position);
        if (direction.sqrMagnitude < 0.0001f)
        {
            return;
        }

        direction.Normalize();
        bool aimingLeft = direction.x < 0f;

        if (characterRenderer != null)
        {
            characterRenderer.flipX = aimingLeft;
        }
        if (weaponVisual != null)
        {
            weaponVisual.SetFacingLeft(aimingLeft);
        }

        weaponPivot.position = transform.position + new Vector3(
            Mathf.Abs(weaponHoldOffset.x) * (aimingLeft ? -1f : 1f),
            weaponHoldOffset.y,
            weaponHoldOffset.z);
        weaponPivot.rotation = Quaternion.Euler(
            0f, 0f, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
    }
}
