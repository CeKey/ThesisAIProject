using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateInteraction : MonoBehaviour {

    Animator animator;
    bool isGateOpen;
    AudioSource audioSource;
    public AudioClip GateOpenSound;


    // Use this for initialization
    void Start () {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        isGateOpen = false;
	}
	
	// Update is called once per frame
	void Update () {
        
	}

    public void ToggleGate()
    {
        //Debug.Log("gate");
        if (!isGateOpen)
        {
            animator.Play("GateOpen");
            audioSource.PlayOneShot(GateOpenSound);
            isGateOpen = true;
        }
        else if (isGateOpen)
        {
            animator.Play("GateClose");
            audioSource.PlayOneShot(GateOpenSound);
            isGateOpen = false;
        }
    }

    public void OpenGate()
    {
        animator.Play("GateOpen");
    }

    public void CloseGate()
    {
        animator.Play("GateClose");
    }
    
}
