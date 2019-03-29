using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameWinUI : MonoBehaviour
{
    public Text GameTime;
    string Minutes, Seconds;
    public Image DifficultyImage;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void Awake()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        GetGameTime();

        DifficultyImage.fillAmount = 0.2f * Player.DifficultyLevel;

        Player.GameOverReason = "";
    }

    void GetGameTime()
    {
        if ((Player.TimePast / 60) < 10)
        {
            Minutes = "Game Time: 0" + ((int)Player.TimePast / 60) + " Min. ";
        }
        else
        {
            Minutes = "Game Time: " + ((int)Player.TimePast / 60) + " Min. ";
        }

        if ((Player.TimePast % 60) < 10)
        {
            Seconds = "0" + ((int)Player.TimePast % 60) + " Sec.";
        }
        else
        {
            Seconds = ((int)Player.TimePast % 60) + " Sec.";
        }
        GameTime.text = Minutes + Seconds;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}