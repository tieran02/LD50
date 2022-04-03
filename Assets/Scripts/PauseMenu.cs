using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{

    public GameObject menu;

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
            Time.timeScale = 0.0f;
        else
            Time.timeScale = 1.0f;

    }

    public void Play()
    {
        ToggleMenu();
    }

    public void Quit()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
