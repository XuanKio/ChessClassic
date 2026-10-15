using System;
using UnityEngine;

// Owns firing cadence and magazine state. ProjectileSpawner creates the actual bullet.
public sealed class WeaponController : MonoBehaviour
{
    [SerializeField] private Transform firePoint;
    [SerializeField] private WeaponData data;
    [SerializeField] private ProjectileSpawner projectileSpawner;

    private bool fireInput;
    private float nextShotTime;
    private float reloadCompleteTime;

    public int CurrentAmmo { get; private set; }
    public int AmmoCapacity => data != null ? data.AmmoCapacity : 0;
    public bool IsReloading { get; private set; }

    public event Action<int, int> OnAmmoChanged;
    public event Action<bool> OnReloadStateChanged;

    private void Awake()
    {
        if (data == null || projectileSpawner == null || !projectileSpawner.IsConfigured)
        {
            Debug.LogError("WeaponController requires WeaponData and a configured ProjectileSpawner.", this);
            enabled = false;
            return;
        }

        CurrentAmmo = AmmoCapacity;
    }

    public void SetFireInput(bool isPressed)
    {
        fireInput = isPressed;
    }

    private void Update()
    {
        if (IsReloading)
        {
            if (Time.time >= reloadCompleteTime) CompleteReload();
            return;
        }

        if (!fireInput || Time.time < nextShotTime) return;

        if (CurrentAmmo == 0) RequestReload();
        else TryFire();
    }

    private void TryFire()
    {
        nextShotTime = Time.time + data.ShotInterval;
        Vector2 direction = transform.right.normalized;
        if (!projectileSpawner.TrySpawn(firePoint, direction, data)) return;

        CurrentAmmo--;
        OnAmmoChanged?.Invoke(CurrentAmmo, AmmoCapacity);
        if (CurrentAmmo == 0) RequestReload();
    }

    public void RequestReload()
    {
        if (!isActiveAndEnabled || data == null || IsReloading || CurrentAmmo >= AmmoCapacity) return;

        IsReloading = true;
        reloadCompleteTime = Time.time + data.ReloadDuration;
        OnReloadStateChanged?.Invoke(true);
    }

    private void CompleteReload()
    {
        CurrentAmmo = AmmoCapacity;
        IsReloading = false;
        OnAmmoChanged?.Invoke(CurrentAmmo, AmmoCapacity);
        OnReloadStateChanged?.Invoke(false);
    }

    public void CancelActions()
    {
        fireInput = false;
        if (!IsReloading) return;

        IsReloading = false;
        OnReloadStateChanged?.Invoke(false);
    }

    private void OnDisable()
    {
        CancelActions();
    }
}
