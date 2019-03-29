using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IngameMenu : MonoBehaviour
{

    GameObject IngameMenuGO;
    public bool isMenuOpen;

    // Use this for initialization
    void Start()
    {
        IngameMenuGO = GameObject.FindGameObjectWithTag("IngameMenuUI");
        IngameMenuGO.SetActive(false);
        isMenuOpen = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            ToggleMenu();
        }

    }

    public void ToggleMenu()
    {
        if (!isMenuOpen)
        { 
            IngameMenuGO.SetActive(true);
            isMenuOpen = true;
            Cursor.visible = true;
            //Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0f;
        }
        else if(isMenuOpen)
        {
            isMenuOpen = false;
            Cursor.visible = false;
            //Cursor.lockState = CursorLockMode.Locked;
            IngameMenuGO.SetActive(false);
            Time.timeScale = 1f;
        }
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
