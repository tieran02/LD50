using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class Clock : MonoBehaviour
{
    private Text clockText;
    public float timer = 32400;
    public float acceleration = 240.0f;
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

        //end shift at 5
        const int startTimeInSeconds = 32400;
        int endTimeInSeconds = startTimeInSeconds + (int)(3600 * 8);
        if(timer >= endTimeInSeconds)
        {
            int currentShift = PlayerPrefs.GetInt("CurrentShift");
            PlayerPrefs.SetInt("CurrentShift", currentShift+1);
            PlayerPrefs.Save();
            SceneManager.LoadScene("GameScene");
        }
    }
}
