using TMPro;
using UnityEngine;

public sealed class AmmoDisplayView : MonoBehaviour
{
    [SerializeField] private WeaponController weapon;
    [SerializeField] private TMP_Text ammoText;

    private void OnEnable()
    {
        if (weapon != null)
        {
            weapon.OnAmmoChanged += Refresh;
            weapon.OnReloadStateChanged += RefreshReload;
        }
        RefreshCurrent();
    }

    private void Start()
    {
        RefreshCurrent();
    }

    private void OnDisable()
    {
        if (weapon != null)
        {
            weapon.OnAmmoChanged -= Refresh;
            weapon.OnReloadStateChanged -= RefreshReload;
        }
    }

    private void RefreshReload(bool reloading) => RefreshCurrent();

    private void RefreshCurrent()
    {
        if (weapon != null) Refresh(weapon.CurrentAmmo, weapon.AmmoCapacity);
    }

    private void Refresh(int current, int capacity)
    {
        if (ammoText == null) return;
        if (weapon != null && weapon.IsReloading) ammoText.SetText("...");
        else ammoText.SetText("{0}/{1}", current, capacity);
    }
}
