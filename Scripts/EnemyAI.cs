using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class EnemyAI : MonoBehaviour {

    Animator AIanimator;

    public float Health;
    public float MaxHealth;
    bool isDead;

    AudioSource audiosource;
    public AudioClip[] DeathSounds;
    bool deathSoundPlayed;

    NavMeshAgent nvagent;
    public Transform[] PathPoints;
    float waitNewPoint;
    public float WaitDirectionChange;

    GameObject player;
    Player playerScript;
    Vector3 playerDirection, playerDirectionLeft, playerDirectionRight;
    bool playerInit;
    public bool WasPlayerSeen;
    public bool PlayerInView;
    public float AIViewAngle;
    public float SightRange;
    bool canSetPlayerInViewFalse;
    bool canSetPlayerInViewTrue;
    RaycastHit targetHit, targetHit2, targetHit3;
    Vector3 lastPlayerPos;
    float curOnPathTime;
    float[] randPathTime = new float[4];
    bool returnToMainPath;
    bool searchForPlayer;
    bool[] searchSectionDone = new bool[4];

    public float MaxHearingDistance;
    public float MaxStepHearingDistance;
    public float MaxWeaponHearingDistance;
    public float MaxImpactHearingDistance;
    public float NoiseCheckReturnTime;
    float waitAfterNoiseCheck;

    bool playSuspiciousSound;
    public AudioClip SuspiciousSound;
    float suspiciousSoundCoolDown;

    public float InstantGameOverDistance;
    float waitReaction;
    public float CurPlayerDistance;

    public AudioSource footStepAudioSource;
    float footstepTime;
    Vector3 oldPos;
    public AudioClip[] footstepSounds;
    public float FootstepLength;


    // Use this for initialization
    void Start () {
        audiosource = GetComponent<AudioSource>();
        nvagent = GetComponent<NavMeshAgent>();
        AIanimator = GetComponent<Animator>();

        isDead = false;
        deathSoundPlayed = false;

        waitNewPoint = WaitDirectionChange; //Get Path directly on start
        Health = MaxHealth;

        playerInit = false;
        PlayerInView = false;
        canSetPlayerInViewFalse = true;
        canSetPlayerInViewTrue = true;
        playerDirection = Vector3.zero;
        playerDirectionLeft = Vector3.zero;
        playerDirectionRight = Vector3.zero;
        lastPlayerPos = Vector3.zero;

        curOnPathTime = 0f;
        waitAfterNoiseCheck = 0f;

        returnToMainPath = true;
        searchForPlayer = false;
        for (int i = 0; i < searchSectionDone.Length; ++i)
            searchSectionDone[i] = false;

        playSuspiciousSound = true;
        waitReaction = 0f;

        footstepTime = 0f;
        oldPos = Vector3.zero;

        
	}
	
	// Update is called once per frame
	void Update () {
        MainPatrol();

        ViewAngle();
        SearchingLastPos();

        DeathCondition();

        NoiseCheck();

        SuspiciousSoundCoolDown();

        WalkAnimation();
        StepSound();
	}

    private void FixedUpdate()
    {
        Init();
        CurPlayerDistance = (player.transform.position - transform.position).magnitude;
    }

    void Init()
    {
        if (!playerInit && GameObject.FindGameObjectWithTag("Player"))
        {
            player = GameObject.FindGameObjectWithTag("Player");
            playerScript = player.GetComponentInChildren<Player>();
            DynamicDifficultyAdjustment();
            playerInit = true;
        }
    }

    void DeathCondition()
    {
        if(Health < 1)
        {
            isDead = true;
            if(deathSoundPlayed == false)
            {
                Destroy(gameObject, 2f);
                nvagent.isStopped = true;
                deathSoundPlayed = true;
                PlayerInView = false;
                WasPlayerSeen = false;
                playerScript.IsPlayerInView = false;
                ++playerScript.EnemyEliminations;
                playerScript.UpdateMaxEnemyUI();
                audiosource.PlayOneShot(DeathSounds[Random.Range(0, DeathSounds.Length)]);
                AIanimator.SetBool("isWalking", false);
                AIanimator.Play("Death");

                nvagent.enabled = false;
            }
            
        }
    }

    public void OnHeadshot()
    {
        Health = 0f;
    }

    void InstantGameOverCheck()
    {
        if (playerDirection.magnitude < InstantGameOverDistance)
        {
            waitReaction += Time.deltaTime;
            if (waitReaction > 1f)
            {
                playerScript.detectionSprite.fillAmount = 1f;
                playerScript.IsPlayerDetected = true;
            }
        }
        else
        {
            waitReaction = 0f;
        }
    }

    void MainPatrol() //Main Patrol Routine
    {
        if (returnToMainPath == true && !isDead)
        {
            waitNewPoint += Time.deltaTime;
            if (waitNewPoint > WaitDirectionChange)
            {
                nvagent.SetDestination(GetRandomPathPoint1());
                if (nvagent.remainingDistance < Random.Range(1f, 15f))
                {
                    waitNewPoint = 0f;
                }
            }
        }
    }

    void ViewAngle() //Move in Player Dir at Active Sight, Start Searching after short contact, go back to patrol if no contact
    {
        playerDirection = player.transform.position - transform.position;
        if (Vector3.Angle(playerDirection, transform.forward * SightRange) < AIViewAngle && Health > 0f  && !isDead) //&& playerScript.makingNoise == false
        {
            if (Physics.Raycast(transform.position, playerDirection, out targetHit))
            {
                Debug.DrawRay(transform.position, playerDirection, Color.green);

                playerDirectionLeft = playerDirection;
                playerDirectionLeft.x -= 0.70f;
                if (Physics.Raycast(transform.position, playerDirectionLeft, out targetHit2))
                {
                    Debug.DrawRay(transform.position, playerDirectionLeft, Color.red);
                }

                playerDirectionRight = playerDirection;
                playerDirectionRight.x += 0.70f;
                if (Physics.Raycast(transform.position, playerDirectionRight, out targetHit3))
                {
                    Debug.DrawRay(transform.position, playerDirectionRight, Color.blue);
                }
                PlayerHitCheck();
            }
        }
    }

    void PlayerHitCheck()       //Active Sight, Seen before, Return to main patrol
    {
        if (targetHit.transform.gameObject.tag == "Player" && playerDirection.magnitude < SightRange || targetHit2.transform.gameObject.tag == "Player" && playerDirection.magnitude < SightRange ||
            targetHit3.transform.gameObject.tag == "Player" && playerDirection.magnitude < SightRange) //Active sight to Player
        {
            lastPlayerPos = player.transform.position;
            lastPlayerPos.y = transform.position.y;
            nvagent.stoppingDistance = 5f;
            nvagent.SetDestination(lastPlayerPos);
            nvagent.speed = 1.1f;

            if (searchForPlayer == true)
                SeenReset();
            searchForPlayer = false;
            returnToMainPath = false;
            PlayerInView = true;
            WasPlayerSeen = true;
            PlaySuspiciousSound();
            InstantGameOverCheck();

            SetPlayerInViewTrue();
            canSetPlayerInViewFalse = true;
            Debug.Log("seen currently");
        }
        else if (targetHit.transform.gameObject.tag != "Player" && targetHit2.transform.gameObject.tag != "Player" && targetHit3.transform.gameObject.tag != "Player" &&
            WasPlayerSeen == true && searchForPlayer == false) // Start searching
        {
            nvagent.SetDestination(lastPlayerPos);
            WasPlayerSeen = false;
            searchForPlayer = true;
            PlayerInView = false;
            randPathTime[0] = Random.Range(4f, 6f);

            SetPlayerInViewFalse();
            canSetPlayerInViewTrue = true;
            Debug.Log("seen before / moving to last seen position");
        }
        else if (WasPlayerSeen == false && PlayerInView == false && searchForPlayer == false) // Return to patrol
        {
            returnToMainPath = true;
            nvagent.stoppingDistance = 0f;
            nvagent.speed = 1.5f;

            canSetPlayerInViewTrue = true;
            SetPlayerInViewFalse();
            Debug.Log("not seen");
        }
    }

    void SearchingLastPos() // Move to 3 points if seen before, return to old Routine if finished
    {

        if (PlayerInView == false && searchForPlayer == true && !isDead)
        {
            Debug.Log("searching");
            curOnPathTime += Time.deltaTime;
            //canSetPlayerInViewTrue = true;
            if (curOnPathTime > randPathTime[0] && searchSectionDone[0] == false)
            {
                nvagent.speed = 1.25f;
                SetRandomSearchPos(-10f, -10f);

                randPathTime[1] = Random.Range(10f, 14f);
                searchSectionDone[0] = true;
                Debug.Log("search1");
            }
            else if (curOnPathTime > randPathTime[1] && searchSectionDone[1] == false && searchSectionDone[0] == true)
            {
                SetRandomSearchPos(-11f, -11f);

                randPathTime[2] = Random.Range(15f, 21f);
                searchSectionDone[1] = true;
                Debug.Log("search2");
            }
            else if (curOnPathTime > randPathTime[2] && searchSectionDone[2] == false && searchSectionDone[1] == true)
            {
                SetRandomSearchPos(-12f, -12f);

                randPathTime[3] = Random.Range(22f, 25f);
                searchSectionDone[2] = true;
                Debug.Log("search3");
            }
            else if (curOnPathTime > randPathTime[3] && searchSectionDone[3] == false && searchSectionDone[2] == true)
            {
                searchSectionDone[3] = true;
                SearchReset();
                Debug.Log("search done / sreset");
            }

        }
        

    }

    void SetRandomSearchPos(float minX, float minZ)
    {
        lastPlayerPos.x += Random.Range(minX, minX *-1);
        lastPlayerPos.z += Random.Range(minZ, minZ*-1);
        nvagent.SetDestination(lastPlayerPos);
    }


    void SearchReset() // Reset search state and variables
    {
        Debug.Log("SearchReset Function");
        canSetPlayerInViewTrue = true;
        canSetPlayerInViewFalse = true;

        searchForPlayer = false;
        WasPlayerSeen = false;
        returnToMainPath = true;
        nvagent.stoppingDistance = 0f;
        nvagent.speed = 1.5f;
        curOnPathTime = 0f;
        for (int i = 0; i < searchSectionDone.Length; i++)
            searchSectionDone[i] = false;
    }

    void SeenReset() // Reset search state and variables if seen
    {
        Debug.Log("SeenReset Function");
        returnToMainPath = true;
        nvagent.stoppingDistance = 5f;
        nvagent.speed = 1.1f;
        curOnPathTime = 0f;
        for (int i = 0; i < searchSectionDone.Length; i++)
            searchSectionDone[i] = false;
    }

    void SetPlayerInViewFalse()
    {
        if(canSetPlayerInViewFalse == true)
        {
            canSetPlayerInViewFalse = false;
            playerScript.IsPlayerInView = false;
        }
    }
    void SetPlayerInViewTrue()
    {
        if (canSetPlayerInViewTrue == true)
        {
            canSetPlayerInViewTrue = false;
            playerScript.IsPlayerInView = true;
        }
    }

    void NoiseCheck() //Move to last Object interacted with
    {
        if (!isDead )//&& searchForPlayer == false)
        {
            //Object Interaction NoiseCheck
            if (playerScript.makingNoise == true && playerScript.LastInteractedGO != null && (playerScript.LastInteractedGO.transform.position - transform.position).magnitude < MaxHearingDistance)
            {
                if (searchForPlayer == true)
                    SearchReset();
                returnToMainPath = false;
                PlaySuspiciousSound();
                nvagent.stoppingDistance = 0f;
                nvagent.SetDestination(playerScript.LastInteractedGO.transform.position);
                Debug.Log("hearing interaction");
            }
            else if (nvagent.remainingDistance < 0.5f && playerScript.makingNoise == false && WasPlayerSeen == false && PlayerInView == false)
            {
                waitAfterNoiseCheck += Time.deltaTime;
                if (waitAfterNoiseCheck > NoiseCheckReturnTime)
                {
                    playerScript.LastInteractedGO = null;
                    waitAfterNoiseCheck = 0f; 
                    returnToMainPath = true;
                }
            }

            //Footstep NoiseCheck
            if (playerScript.FootstepNoise == true && (playerScript.gameObject.transform.position - transform.position).magnitude < MaxStepHearingDistance)
            {
                if (searchForPlayer == true)
                    SearchReset();
                returnToMainPath = false;
                PlaySuspiciousSound();
                nvagent.stoppingDistance = 0f;
                nvagent.SetDestination(playerScript.gameObject.transform.position);
                Debug.Log("hearing steps interaction");
            }
            else if (nvagent.remainingDistance < 2f && playerScript.FootstepNoise == false && WasPlayerSeen == false && PlayerInView == false)
            {
                waitAfterNoiseCheck += Time.deltaTime;
                if (waitAfterNoiseCheck > NoiseCheckReturnTime)
                {
                    waitAfterNoiseCheck = 0f;
                    returnToMainPath = true;
                }
            }

            //Weapon NoiseCheck
            if (playerScript.WeaponNoise == true && (playerScript.gameObject.transform.position - transform.position).magnitude < MaxWeaponHearingDistance)
            {
                if (searchForPlayer == true)
                    SearchReset();
                returnToMainPath = false;
                PlaySuspiciousSound();
                nvagent.stoppingDistance = 0f;
                nvagent.SetDestination(playerScript.gameObject.transform.position);
                Debug.Log("hearing weapon");
            }
            else if (nvagent.remainingDistance < 2f && playerScript.WeaponNoise == false && WasPlayerSeen == false && PlayerInView == false)
            {
                waitAfterNoiseCheck += Time.deltaTime;
                if (waitAfterNoiseCheck > NoiseCheckReturnTime)
                {
                    waitAfterNoiseCheck = 0f;
                    returnToMainPath = true;
                }
            }

            //Impact NoiseCheck
            if (playerScript.ImpactNoise == true && (playerScript.BulletImpactLocation - transform.position).magnitude < MaxImpactHearingDistance)
            {
                if (searchForPlayer == true)
                    SearchReset();
                returnToMainPath = false;
                PlaySuspiciousSound();
                nvagent.stoppingDistance = 1f;
                nvagent.SetDestination(playerScript.BulletImpactLocation);
                playerScript.ImpactNoise = false;
                Debug.Log("hearing bullet impact");
            }
            else if (nvagent.remainingDistance < 2.5f && playerScript.ImpactNoise == false && WasPlayerSeen == false && PlayerInView == false)
            {
                waitAfterNoiseCheck += Time.deltaTime;
                if (waitAfterNoiseCheck > NoiseCheckReturnTime)
                {
                    waitAfterNoiseCheck = 0f;
                    returnToMainPath = true;
                }
            }
        }
    }

    void PlaySuspiciousSound()
    {
        if (playSuspiciousSound == true)
        {
            playSuspiciousSound = false;
            audiosource.PlayOneShot(SuspiciousSound);

        }
    }

    void SuspiciousSoundCoolDown()
    {
        if(playSuspiciousSound == false)
        {
            suspiciousSoundCoolDown += Time.deltaTime;
            if (suspiciousSoundCoolDown > 4f)
            {
                playSuspiciousSound = true;
                suspiciousSoundCoolDown = 0f;
            }
        }
    }

    void DynamicDifficultyAdjustment() //Pre-Calculated before win scene
    {
        if(Player.GameOverReason == "") //Player wins
        {
            if (Player.TimePast == 0f)
            {
                WaitDirectionChange = 4.1f;
                SightRange = 15f;
                MaxHearingDistance = 15f;
                Player.DifficultyLevel = 1;
            }
            else if (Player.TimePast < (float)Player.Difficulty.Hard)
            {
                WaitDirectionChange = 2.1f;
                SightRange = 25f;
                MaxHearingDistance = 25f;
                playerScript.MaxEnemyEliminations = 2;
                Player.DifficultyLevel = 4;
            }
            else if (Player.TimePast < (float)Player.Difficulty.Medium)
            {
                WaitDirectionChange = 3.1f;
                SightRange = 20f;
                MaxHearingDistance = 20f;
                playerScript.MaxEnemyEliminations = 4;
                Player.DifficultyLevel = 3;
            }
            else if (Player.TimePast < (float)Player.Difficulty.Normal)
            {
                WaitDirectionChange = 3.1f;
                SightRange = 20f;
                MaxHearingDistance = 20f;
                playerScript.MaxEnemyEliminations = 5;
                Player.DifficultyLevel = 2;
            }
            else if (Player.TimePast > (float)Player.Difficulty.Normal)
            {
                WaitDirectionChange = 3.1f;
                SightRange = 20f;
                MaxHearingDistance = 20f;
                playerScript.MaxEnemyEliminations = 6;
                Player.DifficultyLevel = 1;
            }
        }
        else //Player lost before
        {

        }
        Player.TimePast = 0f;
    }

    void WalkAnimation()
    {
        if(nvagent.velocity.sqrMagnitude > 0.15f)
        {
            AIanimator.SetBool("isWalking", true);
        }
        else
        {
            AIanimator.SetBool("isWalking", false);
        }
    }

    void StepSound()
    {
        footstepTime += Time.deltaTime;
        if (footstepTime < 0.1f)
        {
            oldPos = transform.position;
        }
        else if (footstepTime > 0.2f)
        {
            if ((transform.position - oldPos).sqrMagnitude > FootstepLength)
            {
                int n = Random.Range(1, footstepSounds.Length);
                footStepAudioSource.clip = footstepSounds[n];
                footStepAudioSource.PlayOneShot(footStepAudioSource.clip);
                footstepSounds[n] = footstepSounds[0];
                footstepSounds[0] = footStepAudioSource.clip;
                footstepTime = 0f;
            }
        }
    }

    Vector3 GetRandomPathPoint1()
    {
        return PathPoints[Random.Range(0, 4)].position;
    }

    public void ReceiveDamage(float damage)
    {
        Health -= damage;
    }
}
