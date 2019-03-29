using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    public Text FailReasonTxt;
    public Text GameTime;
    public Text GameOverTip;
    string Minutes, Seconds;

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
        FailReasonTxt.text = Player.GameOverReason;

        if((Player.TimePast / 60) < 10)
        {
            Minutes = "Game Time: 0" + ((int)Player.TimePast / 60) + " Min. ";
        }
        else
        {
            Minutes = "Game Time: " + ((int)Player.TimePast / 60) + " Min. ";
        }
        if((Player.TimePast % 60) < 10)
        {
            Seconds = "0" + ((int)Player.TimePast % 60) + " Sec.";
        }
        else
        {
            Seconds = ((int)Player.TimePast % 60) + " Sec.";
        }
        GameTime.text = Minutes + Seconds;

        if(FailReasonTxt.text == Player.FailDetectionLevel)
        {
            GameOverTip.text = "Try to sneak near Enemys";
        }
        else if(FailReasonTxt.text == Player.FailMaxEnemys)
        {
            GameOverTip.text = "Do not eliminate more than the given maximum of enemys";
        }

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
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