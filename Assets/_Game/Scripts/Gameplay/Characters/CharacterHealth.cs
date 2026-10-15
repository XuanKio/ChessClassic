using System;
using UnityEngine;

public sealed class CharacterHealth : MonoBehaviour, IDamageable
{
    [SerializeField, Min(1f)] private float maxHealth = 100f;

    private float currentHealth;
    private bool initialized;

    public float CurrentHealth
    {
        get
        {
            EnsureInitialized();
            return currentHealth;
        }
    }

    public float MaxHealth => maxHealth;
    public bool IsDead => CurrentHealth <= 0f;

    public event Action<float, float> OnHealthChanged;
    public event Action OnDamaged;
    public event Action OnDied;

    private void Awake()
    {
        EnsureInitialized();
    }

    // Health can be read in another component's OnEnable, before our Awake runs.
    private void EnsureInitialized()
    {
        if (initialized) return;
        currentHealth = maxHealth;
        initialized = true;
    }

    public void TakeDamage(float damage)
    {
        if (IsDead || damage <= 0f)
        {
            return;
        }

        currentHealth = Mathf.Max(0f, CurrentHealth - damage);
        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
        OnDamaged?.Invoke();

        if (IsDead)
        {
            OnDied?.Invoke();
        }
    }
}
