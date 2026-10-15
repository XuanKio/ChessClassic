using UnityEngine;

// Shared tuning only. Each WeaponController owns its current ammo and reload timer.
[CreateAssetMenu(fileName = "WeaponData", menuName = "Game/Weapons/Weapon Data")]
public sealed class WeaponData : ScriptableObject
{
    [Header("Firing")]
    [SerializeField, Min(0.01f)] private float shotInterval = 0.18f;
    [SerializeField, Min(0f)] private float bulletSpeed = 9f;
    [SerializeField, Min(0.01f)] private float bulletLifetime = 2f;

    [Header("Magazine")]
    [SerializeField, Min(1)] private int ammoCapacity = 30;
    [SerializeField, Min(0.01f)] private float reloadDuration = 1.5f;

    public float ShotInterval => shotInterval;
    public float BulletSpeed => bulletSpeed;
    public float BulletLifetime => bulletLifetime;
    public int AmmoCapacity => ammoCapacity;
    public float ReloadDuration => reloadDuration;
}
