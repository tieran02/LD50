using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
public class Clock : MonoBehaviour
{
    private Text clockText;
    public float timer = 32400;
    public float acceleration = 120.0f;
    private int hours;
    private int minutes;

    // Start is called before the first frame update
    void Start()
    {
        clockText = GetComponent<Text>();
        minutes = (((int)timer)%3600)/60;
        hours = (((int)timer)/3600)%24;
        clockText.text = hours.ToString().PadLeft(2,'0')+ ":" + minutes.ToString().PadLeft(2,'0');
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime* acceleration;
        minutes = (((int)timer)%3600)/60;
        hours = (((int)timer)/3600)%24;
        clockText.text = hours.ToString().PadLeft(2,'0')+ ":" + minutes.ToString().PadLeft(2,'0');
    }
}
