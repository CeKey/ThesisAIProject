using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class PlayerController : MonoBehaviour {

    public float walkSpeed;
    public float SlowWalkSpeed;
    Vector3 moveDirection;
    Rigidbody rigidb;
    MouseLook mlook;
    Camera camera;
    AudioSource audioSource;
    Animator animator;
    public AudioClip[] footstepSounds;
    public float FootstepLength;
    public static bool isWalking;
    Player playerScript;

    float footstepTime;
    Vector3 oldPos;
    float oldWalkSpeed;

	// Use this for initialization
	void Start () {

        rigidb = GetComponent<Rigidbody>();
        mlook = GetComponentInChildren<MouseLook>();
        camera = GetComponentInChildren<Camera>();
        audioSource = GetComponent<AudioSource>();
        animator = GetComponentInChildren<Animator>();
        playerScript = GetComponentInChildren<Player>();

        isWalking = false;
        oldWalkSpeed = walkSpeed;
        mlook.Init(transform, camera.transform);
        footstepTime = 0f;
    }
	
	// Update is called once per frame
	void Update () {
        float horizontalMovement = Input.GetAxisRaw("Horizontal");
        float verticalMovement = Input.GetAxisRaw("Vertical");

        moveDirection = (horizontalMovement * transform.right + verticalMovement * transform.forward).normalized;
        rigidb.velocity = moveDirection * walkSpeed * Time.deltaTime;

        mlook.LookRotation(transform, camera.transform);

        footstepTime += Time.deltaTime;
        if(footstepTime < 0.1f)
        {
            oldPos = transform.position;
        }
        else if(footstepTime > 0.2f)
        {
            if((transform.position - oldPos).sqrMagnitude > FootstepLength)
            {
                FootStepSound();
                footstepTime = 0f;
            }
        }

        PlayWalkAnimation();
    }

    void FixedUpdate()
    {
        
    }

    void FootStepSound()
    {
        int n = Random.Range(1, footstepSounds.Length);
        audioSource.clip = footstepSounds[n];
        audioSource.PlayOneShot(audioSource.clip);
        footstepSounds[n] = footstepSounds[0];
        footstepSounds[0] = audioSource.clip;
    }

    void CheckIsWalking()
    {
        if(GetComponent<Rigidbody>().velocity.magnitude < 0.01f)
        {
            isWalking = false;
        }
        else
        {
            isWalking = true;
        }
    }

    void PlayWalkAnimation()
    {
        if (Input.GetButton("Horizontal") && !Input.GetButton("SlowWalk") || Input.GetButton("Vertical") && !Input.GetButton("SlowWalk"))
        {
            animator.SetBool("isWalking", true);
            animator.SetBool("isShooting", false);
            playerScript.FootstepNoise = true;
        }
        else if (!Input.GetButton("Horizontal") || !Input.GetButton("Vertical"))
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isSlowWalking", false);
            playerScript.FootstepNoise = false;
        }
        CheckIsWalking();
        PlaySlowWalkAnimation();
    }

    void PlaySlowWalkAnimation()
    {
        if(Input.GetButton("SlowWalk") && Input.GetButton("Horizontal") || Input.GetButton("SlowWalk") && Input.GetButton("Vertical"))
        {
            animator.SetBool("isSlowWalking", true);
            animator.SetBool("isShooting", false);
            walkSpeed = SlowWalkSpeed;
            playerScript.FootstepNoise = false;
        }
        else if (!Input.GetButton("SlowWalk"))
        {
            animator.SetBool("isSlowWalking", false);
            walkSpeed = oldWalkSpeed;
        }
    }

    
}
