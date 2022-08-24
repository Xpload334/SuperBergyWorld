using UnityEngine;
using UnityEngine.UI;

public class CombatPointsBar : MonoBehaviour
{
    private CombatPointsSystem _combatPointsSystem;
    private Slider _slider;

    private void Setup(CombatPointsSystem combatPointsSystem)
    {
        this._combatPointsSystem = combatPointsSystem;
        
        _slider = GetComponent<Slider>();
    }

    public void ChangeCP()
    {
        _slider.value = _combatPointsSystem.GetCP();
        _slider.maxValue = _combatPointsSystem.GetCPPercent();
    }

    private void OnEnable()
    {
        CombatPointsSystem.OnCPChanged += ChangeCP;
    }

    private void OnDisable()
    {
        HealthSystem.OnHealthChanged -= ChangeCP;
    }
}
