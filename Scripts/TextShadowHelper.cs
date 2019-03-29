using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextShadowHelper : MonoBehaviour {

    public Text mainText;
    Text shadowText;

	// Use this for initialization
	void Start () {
        shadowText = GetComponent<Text>();
        shadowText.text = mainText.text;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void FixedUpdate()
    {
        shadowText.text = mainText.text;
    }
}
