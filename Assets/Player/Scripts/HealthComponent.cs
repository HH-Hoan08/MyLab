using System;
using UnityEngine;

public class HealthComponent : MonoBehaviour
{
    #region Setting Health
    [SerializeField] private float maxHealth = 100;
    private float currentHealth;
    #endregion

    #region Action
    public event Action<float, float> OnHealthChanged;
    public event Action OnDeath;
    #endregion

    private void Awake()
    {
        currentHealth = maxHealth;

    }

    private void TakeDamage(float amount)
    {
        if (currentHealth == 0) return;
        currentHealth = Math.Clamp(currentHealth - amount, 0f, maxHealth);
        if (currentHealth <= 0) OnDeath.Invoke();   
    }
}