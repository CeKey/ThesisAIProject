using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorInteraction : MonoBehaviour {

    Animator animator;
    bool isDoorOpen;
    AudioSource audioSource;
    public AudioClip DoorOpenSound;
    public AudioClip DoorCloseSound;

    // Use this for initialization
    void Start () {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        isDoorOpen = false;
    }
	
	// Update is called once per frame
	void Update () {

	}
    public void ToggleDoor()
    {
        if (!isDoorOpen)
        {
            animator.Play("OpenDoor");
            audioSource.PlayOneShot(DoorOpenSound);
            isDoorOpen = true;
        }
        else if (isDoorOpen)
        {
            animator.Play("CloseDoor");
            audioSource.PlayOneShot(DoorCloseSound);
            isDoorOpen = false;
        }
    }

    public void OpenDoor()
    {
        animator.Play("OpenDoor");
    }

    public void CloseDoor()
    {
        animator.Play("CloseDoor");
    }
}
