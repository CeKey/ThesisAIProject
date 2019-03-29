using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour {
    public float timeToFadeOut;
    public GameObject TutorialText1, TutorialText2, TutorialText3, TutorialText4;

    float waitBeforeShowTut;

    // Use this for initialization
    void Start()
    {

        if (Player.ShowTutorial == true)
        {
            waitBeforeShowTut = 0f;
            TutorialText1.SetActive(false);

            Color tmpcolor = TutorialText1.GetComponent<Image>().color;
            tmpcolor.a = 1f;
            TutorialText1.GetComponent<Image>().color = tmpcolor;
            Color tmpcolortxt = TutorialText1.GetComponentInChildren<Text>().color;
            tmpcolortxt.a = 1f;
            TutorialText1.GetComponentInChildren<Text>().color = tmpcolortxt;

            Color tmpcolor2 = TutorialText2.GetComponent<Image>().color;
            tmpcolor2.a = 0f;
            TutorialText2.GetComponent<Image>().color = tmpcolor2;
            Color tmpcolor2txt = TutorialText2.GetComponentInChildren<Text>().color;
            tmpcolor2txt.a = 0f;
            TutorialText2.GetComponentInChildren<Text>().color = tmpcolor2txt;

            Color tmpcolor3 = TutorialText3.GetComponent<Image>().color;
            tmpcolor3.a = 0f;
            TutorialText3.GetComponent<Image>().color = tmpcolor3;
            Color tmpcolor3txt = TutorialText3.GetComponentInChildren<Text>().color;
            tmpcolor3txt.a = 0f;
            TutorialText3.GetComponentInChildren<Text>().color = tmpcolor3txt;

            Color tmpcolor4 = TutorialText4.GetComponent<Image>().color;
            tmpcolor4.a = 0f;
            TutorialText4.GetComponent<Image>().color = tmpcolor4;
            Color tmpcolor4txt = TutorialText4.GetComponentInChildren<Text>().color;
            tmpcolor4txt.a = 0f;
            TutorialText4.GetComponentInChildren<Text>().color = tmpcolor4txt;
        }
        else
        {
            Color tmpcolor = TutorialText1.GetComponent<Image>().color;
            tmpcolor.a = 0f;
            TutorialText1.GetComponent<Image>().color = tmpcolor;
            Color tmpcolortxt = TutorialText1.GetComponentInChildren<Text>().color;
            tmpcolortxt.a = 0f;
            TutorialText1.GetComponentInChildren<Text>().color = tmpcolortxt;

            Color tmpcolor2 = TutorialText2.GetComponent<Image>().color;
            tmpcolor2.a = 0f;
            TutorialText2.GetComponent<Image>().color = tmpcolor2;
            Color tmpcolor2txt = TutorialText2.GetComponentInChildren<Text>().color;
            tmpcolor2txt.a = 0f;
            TutorialText2.GetComponentInChildren<Text>().color = tmpcolor2txt;

            Color tmpcolor3 = TutorialText3.GetComponent<Image>().color;
            tmpcolor3.a = 0f;
            TutorialText3.GetComponent<Image>().color = tmpcolor3;
            Color tmpcolor3txt = TutorialText3.GetComponentInChildren<Text>().color;
            tmpcolor3txt.a = 0f;
            TutorialText3.GetComponentInChildren<Text>().color = tmpcolor3txt;

            Color tmpcolor4 = TutorialText4.GetComponent<Image>().color;
            tmpcolor4.a = 0f;
            TutorialText4.GetComponent<Image>().color = tmpcolor4;
            Color tmpcolor4txt = TutorialText4.GetComponentInChildren<Text>().color;
            tmpcolor4txt.a = 0f;
            TutorialText4.GetComponentInChildren<Text>().color = tmpcolor4txt;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Player.ShowTutorial == true)
        {
            waitBeforeShowTut += Time.deltaTime;
            if (waitBeforeShowTut > 3f)
            {
                TutorialText1.SetActive(true);
                timeToFadeOut -= Time.deltaTime;
                if (timeToFadeOut <= 0)
                {
                    TutorialText1.GetComponent<Image>().CrossFadeAlpha(0.0f, 0.15f, false);
                    TutorialText1.GetComponentInChildren<Text>().CrossFadeAlpha(0.0f, 0.15f, false);

                    if (timeToFadeOut <= -8f) // next tutorial text
                    {
                        Color tmpcolor2 = TutorialText2.GetComponent<Image>().color;
                        tmpcolor2.a = 1f;
                        TutorialText2.GetComponent<Image>().color = tmpcolor2;

                        Color tmpcolor2txt = TutorialText2.GetComponentInChildren<Text>().color;
                        tmpcolor2txt.a = 1f;
                        TutorialText2.GetComponentInChildren<Text>().color = tmpcolor2txt;

                        if (timeToFadeOut <= -12f) //fade out text2
                        {
                            TutorialText2.GetComponent<Image>().CrossFadeAlpha(0.0f, 0.15f, false);
                            TutorialText2.GetComponentInChildren<Text>().CrossFadeAlpha(0.0f, 0.15f, false);
                        }

                        if (timeToFadeOut <= -16f) // next tutorial text 3
                        {
                            Color tmpcolor3 = TutorialText3.GetComponent<Image>().color;
                            tmpcolor3.a = 1f;
                            TutorialText3.GetComponent<Image>().color = tmpcolor3;

                            Color tmpcolor3txt = TutorialText3.GetComponentInChildren<Text>().color;
                            tmpcolor3txt.a = 1f;
                            TutorialText3.GetComponentInChildren<Text>().color = tmpcolor3txt;

                            if (timeToFadeOut <= -20f) //fade out text3
                            {
                                TutorialText3.GetComponent<Image>().CrossFadeAlpha(0.0f, 0.15f, false);
                                TutorialText3.GetComponentInChildren<Text>().CrossFadeAlpha(0.0f, 0.15f, false);
                            }

                            if (timeToFadeOut <= -24f) // next tutorial text 4
                            {
                                Color tmpcolor4 = TutorialText4.GetComponent<Image>().color;
                                tmpcolor4.a = 1f;
                                TutorialText4.GetComponent<Image>().color = tmpcolor4;

                                Color tmpcolor4txt = TutorialText3.GetComponentInChildren<Text>().color;
                                tmpcolor4txt.a = 1f;
                                TutorialText4.GetComponentInChildren<Text>().color = tmpcolor4txt;

                                if (timeToFadeOut <= -28f) //fade out text4
                                {
                                    TutorialText4.GetComponent<Image>().CrossFadeAlpha(0.0f, 0.15f, false);
                                    TutorialText4.GetComponentInChildren<Text>().CrossFadeAlpha(0.0f, 0.15f, false);
                                    Player.ShowTutorial = false;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
