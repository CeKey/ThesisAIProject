using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    public GameObject OptionsGO;
    public bool OptionsOpen;
    public Text displayGraphicsLevel;
    public Toggle TutorialCheckbox;

	// Use this for initialization
	void Start () {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        OptionsOpen = false;
        displayGraphicsLevel.text = "";
        for (int i = -1; i < QualitySettings.GetQualityLevel(); i++)
            displayGraphicsLevel.text = displayGraphicsLevel.text + "●";

        OptionsGO.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {

	}

    public void ToggleOptions()
    {
        if(OptionsOpen)
        {
            OptionsGO.SetActive(false);
            OptionsOpen = false;
        }
        else if (!OptionsOpen)
        {
            OptionsGO.SetActive(true);
            OptionsOpen = true;
        }
    }

    public void GraphicsIncrease()
    {
        if(QualitySettings.GetQualityLevel() < 4)
        {
            displayGraphicsLevel.text = displayGraphicsLevel.text + "●";
            QualitySettings.IncreaseLevel(true);
        }
    }

    public void GraphicsDecrease()
    {
        if (QualitySettings.GetQualityLevel() > 0)
        {
            displayGraphicsLevel.text = displayGraphicsLevel.text.Remove(displayGraphicsLevel.text.Length - 1);
            QualitySettings.DecreaseLevel(true);
        }
    }

    public void ToggleShowTutorial()
    {
        if(TutorialCheckbox.isOn)
        {
            Player.ShowTutorial = true;
        }
        else
        {
            Player.ShowTutorial = false;
        }
    }


    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
