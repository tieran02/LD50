using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StressBar : MonoBehaviour
{

    public Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    public void Initialize()
    {
        slider.maxValue = 1;
        slider.minValue = 0;
        slider.value = 0.082f;
    }
    public void setStress(int stress)
    {
        //Map stress value from 0-> 100 to the range of values that the stress bar is visible for.
        slider.value = ((float)stress)*((0.755f-0.082f)/(float)100);
    }
}
