using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class ShowUnitStat : MonoBehaviour
{
    [SerializeField] 
    public Slider slider;
    
    [SerializeField]
    protected GameObject unit;
    void Start () 
    {
        
    }
    void Update () 
    {
        if(this.unit) 
        {
            float newValue = this.newStatValue();
            slider.value = newValue;

            float newMaxValue = this.newMaxStateValue();
            slider.maxValue = newMaxValue;
            /*
            float newScale = (this.initialScale.x * newValue) / this.maxValue;
            this.gameObject.transform.localScale = new Vector2(newScale, this.initialScale.y);
            */
        }
    }
    
    /*
     * Change the unit being displayed
     */
    public void changeUnit (GameObject newUnit)
    {
        this.unit = newUnit;
    }
    
    /*
     * Get new state value
     */
    abstract protected float newStatValue();

    /*
     * Get new max state value
     */
    abstract protected float newMaxStateValue();
}
