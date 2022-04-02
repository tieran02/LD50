using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StressBar : MonoBehaviour
{

    private Slider slider;

    public void SetMaxStress(int stress)
    {
        slider.maxValue = stress;
        slider.value = stress;
    }
    public void setStress(int stress)
    {
        slider.value = stress;
    }
}
