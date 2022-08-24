using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private HealthSystem _healthSystem;
    private Slider _slider;

    public void Setup(HealthSystem healthSystem)
    {
        this._healthSystem = healthSystem;
        
        _slider = GetComponent<Slider>();
        ChangeHealth(); //Initialise health
    }

    public void ChangeHealth()
    {
        _slider.value = _healthSystem.GetHealth();
        _slider.maxValue = _healthSystem.GetMaxHealth();
    }

    private void OnEnable()
    {
        HealthSystem.OnHealthChanged += ChangeHealth;
    }

    private void OnDisable()
    {
        HealthSystem.OnHealthChanged -= ChangeHealth;
    }
}
