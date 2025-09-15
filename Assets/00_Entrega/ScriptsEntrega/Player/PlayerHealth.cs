using System;
using UnityEngine;

[DisallowMultipleComponent]
public class PlayerHealth : MonoBehaviour
{
    [SerializeField, Min(1)] private int maxHealth = 100;
    [SerializeField] private int currentHealth;

    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;
    public bool IsDead => currentHealth <= 0;

    public event Action<int, int> OnHealthChanged; // (current, max)
    public event Action OnDied;

    void Awake()
    {
        if (currentHealth <= 0) currentHealth = maxHealth;
        NotifyHealthChanged();
    }

    public void SetMaxHealth(int value, bool refill = true)
    {
        maxHealth = Mathf.Max(1, value);
        if (refill) currentHealth = maxHealth;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        NotifyHealthChanged();
        if (IsDead) RaiseDeath();
    }

    public void TakeDamage(int amount)
    {
        if (IsDead) return;
        if (amount <= 0) return;

        currentHealth = Mathf.Max(0, currentHealth - amount);
        NotifyHealthChanged();
        if (currentHealth == 0) RaiseDeath();
    }

    public void Heal(int amount)
    {
        if (IsDead) return;
        if (amount <= 0) return;

        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        NotifyHealthChanged();
    }

    public void Kill()
    {
        if (IsDead) return;
        currentHealth = 0;
        NotifyHealthChanged();
        RaiseDeath();
    }

    void NotifyHealthChanged() => OnHealthChanged?.Invoke(currentHealth, maxHealth);

    void RaiseDeath()
    {
        // Garantiza una sola notificación
        if (OnDied != null) OnDied.Invoke();
    }
}
