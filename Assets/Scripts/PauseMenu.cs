using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{

    public GameObject menu;
    public Button topButton;

    // Start is called before the first frame update
    void Start()
    {
        menu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Cancel"))
        {
            ToggleMenu();
        }
    }

    void ToggleMenu()
    {
        menu.SetActive(!menu.activeSelf);
        bool active = menu.activeSelf;

        if (active)
        {
            Time.timeScale = 0.0f;
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(topButton.gameObject);
        }
        else
        {
            Time.timeScale = 1.0f;
            EventSystem.current.SetSelectedGameObject(null);
        }

    }

    public void Play()
    {
        ToggleMenu();
    }

    public void Quit()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("MainMenu");
    }
}
