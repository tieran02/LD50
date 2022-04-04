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
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (eventSystem)
            eventSystem.enabled = false;
        if (Input.anyKeyDown)
        {
            if(eventSystem)
                eventSystem.enabled = true;
            gameObject.SetActive(false);
        }
    }
}
