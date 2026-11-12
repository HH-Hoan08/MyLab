using UnityEngine;

public class WaterManager : MonoBehaviour
{
    [Header("Ripple Settings")]
    [SerializeField] private RippleEffect rippleEffect;

    [Header("Interaction Filter")]
    [SerializeField] private bool allowRipple = true;
    [SerializeField] private LayerMask allowedLayers;

    private void Awake()
    {
        if (rippleEffect == null)
        {
            rippleEffect = GetComponentInChildren<RippleEffect>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!allowRipple) return;
        if ((allowedLayers.value & (1 << other.gameObject.layer)) == 0)
        {
            return;
        }
    }
    public void SetRippleEnabled(bool state)
    {
        allowRipple = state;
        if (rippleEffect != null)
        {
            if (state) rippleEffect.StartSimulation();
            else rippleEffect.StopSimulation();
        }
    }
}