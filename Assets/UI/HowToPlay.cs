using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HowToPlay : MonoBehaviour
{
    EventSystem eventSystem;

    private void Awake()
    {
        eventSystem = FindObjectOfType<EventSystem>();
        int currentShift = PlayerPrefs.GetInt("CurrentShift");
        if(currentShift == 0)
            gameObject.SetActive(true);
        else
            gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        Time.timeScale = 0.0f;
        if (eventSystem)
            eventSystem.enabled = false;
        if (Input.anyKeyDown)
        {
            if(eventSystem)
                eventSystem.enabled = true;

            gameObject.SetActive(false);
            if (!FindObjectOfType<PauseMenu>().menu.activeSelf)
                Time.timeScale = 1.0f;
        }
    }
}
