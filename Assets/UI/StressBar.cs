using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StressBar : MonoBehaviour
{

    public Slider slider;

    public void SetMaxStress(int stress)
    {
        slider.maxValue = stress;
        slider.value = 0;
    }
    public void setStress(int stress)
    {
        slider.value = stress;
    }
}
