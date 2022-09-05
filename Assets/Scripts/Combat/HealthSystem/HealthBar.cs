using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private HealthSystem _healthSystem;
    [SerializeField]
    private Slider slider;
    [SerializeField]
    private TMP_Text tmpText;

    public void Setup(HealthSystem healthSystem)
    {
        this._healthSystem = healthSystem;
        
        slider = GetComponent<Slider>();
        ChangeHealth(); //Initialise health
    }

    public void ChangeHealth()
    {
        slider.value = _healthSystem.GetHealth();
        slider.maxValue = _healthSystem.GetMaxHealth();

        tmpText.text = _healthSystem.GetHealth().ToString();
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
