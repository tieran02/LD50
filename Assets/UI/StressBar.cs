using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StressBar : MonoBehaviour
{

    public Image insideImage;

    private void Awake()
    {
        insideImage = GetComponent<Image>();
    }

    public void Initialize()
    {
        insideImage.fillAmount = 0.0f;
    }
    public void setStress(float stress)
    {
        //Map stress value from 0-> 100 to the range of values that the stress bar is visible for.
        insideImage.fillAmount = stress/100.0f;
    }
}
