using UnityEngine;
using UnityEngine.UI;

public class StateUI : MonoBehaviour
{
    [SerializeField] private HealthComponent health;
    [SerializeField] private StaminaComponet stamina;
    [SerializeField] private Image healthBarFill;
    [SerializeField] private Image staminaBarFill;

    private void OnEnable()
    {
        // Luôn đăng ký lắng nghe ở OnEnable
        if (health != null)
            health.OnHealthChanged += UpdateHealthBar;
        if (stamina != null)
            stamina.OnStaminaBar += UpdateStaminaBar;
    }

    private void OnDisable()
    {
        // Bắt buộc hủy đăng ký ở OnDisable để tránh rò rỉ bộ nhớ (Memory Leak)
        if (health != null)
            health.OnHealthChanged -= UpdateHealthBar;
        if (stamina != null)
            stamina.OnStaminaBar -= UpdateStaminaBar;
    }

    private void UpdateHealthBar(float current, float max)
    {
        healthBarFill.fillAmount = current / max;
    }

    private void UpdateStaminaBar(float current, float max)
    {
        staminaBarFill.fillAmount = current / max;
    }
}