using System;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class StaminaComponet : MonoBehaviour
{

    public event Action<float, float> OnStaminaBar;
    [SerializeField] public float maxStamina = 100f;
    public float currentStamina;

    private void Awake()
    {
        currentStamina = maxStamina;
    }

    public void TakeStamina(float amount)
    {
        if (currentStamina == 0) return;
        currentStamina = Math.Clamp(currentStamina - amount, 0, maxStamina);
        OnStaminaBar?.Invoke(currentStamina, maxStamina);
    }

    public void RecoveryStamina(float amount) 
    {
        currentStamina = Math.Clamp(currentStamina + amount, 0, maxStamina);
        OnStaminaBar?.Invoke(currentStamina, maxStamina);
    }
}
