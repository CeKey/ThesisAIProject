using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{

    public bool CanInteract;
    Camera camera;
    AudioSource audiosource;

    public bool ObjectCollected;
    GameObject UIObjectCollected;

    public float InteractionDistance;
    float waitToggle;
    RaycastHit rayHit;

    Weapon weaponScript;
    RectTransform BulletImage;
    Vector2 UIBulletSize;

    public bool IsPlayerInView;
    public bool IsPlayerDetected;
    public float DetectionSpeed;
    public float DetectionCoolDownSpeed;
    public Image detectionSprite;
    float detectionReduceTime;
    bool playDetectionSound;
    public AudioClip DetectionSound;
    bool playAlarmSound;
    public AudioClip AlarmSound;

    public bool makingNoise;
    float noiseDuration;
    public float MaxNoiseDuration;
    public GameObject LastInteractedGO = null;

    public int EnemyEliminations;
    public int MaxEnemyEliminations;
    Text UIEnemyEliminations;

    public static string GameOverReason;
    public static string FailMaxEnemys;
    public static string FailDetectionLevel;
    float waitGameOver;

    public static float TimePast;
    string Minutes, Seconds;
    Text UITimePast;

    public Transform MarkPoint;
    public float MarkPointRadius;
    public float ObjectReleaseSpeed;
    public Image progressCircle;
    Text UIProgressPercent;
    float waitWin;
    bool GameWon;

    public static bool ShowTutorial = true;
    public static byte DifficultyLevel;

    public bool FootstepNoise;

    public bool WeaponNoise;
    public float MaxWeaponNoiseDuration;
    float weaponNoiseDuration;
    public Vector3 BulletImpactLocation;
    public bool ImpactNoise;


    public enum Difficulty {Normal=100, Medium=60, Hard=45};

    // Use this for initialization
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        camera = GetComponent<Camera>();
        audiosource = GetComponent<AudioSource>();
        weaponScript = GetComponentInChildren<Weapon>();

        UIBulletSize = new Vector2(395f, 999f);

        waitToggle = 0f;

        ObjectCollected = false;
        CanInteract = true;

        makingNoise = false;

        IsPlayerInView = false;
        IsPlayerDetected = false;
        detectionSprite = GameObject.FindGameObjectWithTag("UIDetector").GetComponent<Image>();
        detectionSprite.fillAmount = 0f;
        detectionReduceTime = 0f;
        playDetectionSound = true;
        playAlarmSound = true;

        UIEnemyEliminations = GameObject.FindGameObjectWithTag("MaxEnemysUI").GetComponent<Text>();
        UIEnemyEliminations.text = UIEnemyEliminations.text = "" + EnemyEliminations + "/" + MaxEnemyEliminations;

        BulletImage = GameObject.FindGameObjectWithTag("BulletsUI").GetComponent<RectTransform>();

        GameOverReason = null;
        FailMaxEnemys = "Maximum of eliminations exceeded";
        FailDetectionLevel = "An Enemy detected you";
        waitGameOver = 0f;

        //TimePast = 0f;
        UITimePast = GameObject.FindGameObjectWithTag("TimePastUI").GetComponent<Text>();

        progressCircle = GameObject.FindGameObjectWithTag("ProgressCircleUI").GetComponent<Image>();
        progressCircle.fillAmount = 0f;
        UIProgressPercent = GameObject.FindGameObjectWithTag("ProgressPercentUI").GetComponent<Text>();
        UIProgressPercent.text = "";
        waitWin = 0f;
        GameWon = false;

        UIObjectCollected = GameObject.FindGameObjectWithTag("UIObjectCollected");
        UIObjectCollected.SetActive(false);

        FootstepNoise = false;
        WeaponNoise = false;
        weaponNoiseDuration = 0;
        ImpactNoise = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(!IsPlayerDetected && !GameWon)
        TimePast += Time.deltaTime;

        if (Input.GetButtonDown("Action"))
        {
            if(CanInteract)
            {
                ObjectInteraction();
            }
        }

        NoiseCoolDown();
        WeaponNoiseCoolDown();

        InteractionCoolDown();

        PlayerDetection();

        GameOverCheck();
        GameWinCheck();
        
    }

    void FixedUpdate()
    {
        UpdateBulletUI();
        UpdateObjectCollectedUI();

        UpdateGameTime();

        ItemDeliveryUpdate();
        ProgressPercentUpdate();
        
    }

    private void Awake()
    {
        //TimePast = 0f;
    }


    void PlayerDetection()
    {
        if (IsPlayerInView)
        {
            if (detectionSprite.fillAmount < 1f) //Increase Detection Level
            {
                detectionSprite.fillAmount += DetectionSpeed * Time.deltaTime;
                if (detectionSprite.fillAmount > 0.5f && playDetectionSound == true)
                {
                    audiosource.PlayOneShot(DetectionSound);
                    playDetectionSound = false;
                }
            }
            if(detectionSprite.fillAmount == 1f)
            {
                IsPlayerDetected = true;
            }
        }
        else if (!IsPlayerInView)
        {
            detectionReduceTime += Time.deltaTime;
            if (detectionReduceTime > 4f)
            {
                if (detectionSprite.fillAmount > 0.0f && !IsPlayerDetected) //Decrease Detection Level
                {
                    detectionSprite.fillAmount -= DetectionCoolDownSpeed * Time.deltaTime;
                    if(playDetectionSound == false)
                    {
                        playDetectionSound = true;
                    }
                }
                else
                {
                    detectionReduceTime = 0f;
                }
            }
        }
    }

    void ObjectInteraction()
    {
        Debug.DrawRay(camera.transform.position, camera.transform.forward * InteractionDistance, Color.green);
        if (Physics.Raycast(camera.transform.position, camera.transform.forward * InteractionDistance, out rayHit))
        {
            if (rayHit.transform.tag == "Gate")
            {
                GateInteraction();
                LastInteractedGO = rayHit.transform.gameObject;
            }
            else if (rayHit.transform.tag == "Door")
            {
                DoorInteraction();
                LastInteractedGO = rayHit.transform.gameObject;
            }
        }
    }

    void InteractionCoolDown()
    {
        if (!CanInteract)
        {
            waitToggle += Time.deltaTime;
            if (waitToggle >= 3.5f)
            {
                CanInteract = true;
                waitToggle = 0f;
            }
        }
    }

    void NoiseCoolDown()
    {
        if (makingNoise == true)
        {
            noiseDuration += Time.deltaTime;
            if (noiseDuration >= MaxNoiseDuration)
            {
                makingNoise = false;
                noiseDuration = 0f;
            }
        }
    }

    void WeaponNoiseCoolDown()
    {
        if (WeaponNoise == true)
        {
            weaponNoiseDuration += Time.deltaTime;
            if (weaponNoiseDuration >= MaxWeaponNoiseDuration)
            {
                WeaponNoise = false;
                weaponNoiseDuration = 0f;
            }
        }
    }

    void GateInteraction()
    {
        rayHit.transform.gameObject.GetComponent<GateInteraction>().ToggleGate();

        makingNoise = true;
        CanInteract = false;
    }

    void DoorInteraction()
    {
        rayHit.transform.gameObject.GetComponent<DoorInteraction>().ToggleDoor();

        makingNoise = true;
        CanInteract = false;
    }

    void UpdateBulletUI()
    {
        if (BulletImage.sizeDelta.x != (UIBulletSize.x * weaponScript.Bullets))
        {
            Vector2 tmpVec = BulletImage.sizeDelta;
            tmpVec.x = UIBulletSize.x * weaponScript.Bullets;
            BulletImage.sizeDelta = tmpVec;
        }
    }

    void UpdateObjectCollectedUI()
    {
        if(ObjectCollected)
        {
            UIObjectCollected.SetActive(true);
        }
        else if (!ObjectCollected)
        {
            UIObjectCollected.SetActive(false);
        }
    }

    void ItemDeliveryUpdate()
    {
        if (ObjectCollected == true && (transform.position - MarkPoint.transform.position).magnitude < MarkPointRadius)
        {
            progressCircle.fillAmount += Time.deltaTime * ObjectReleaseSpeed;
        }
        else
        {
            progressCircle.fillAmount = 0f;
        }
    }

    void ProgressPercentUpdate()
    {
        if (progressCircle.fillAmount < 0.01f)
        {
            UIProgressPercent.text = "";
        }
        else if (progressCircle.fillAmount > 0.00f)
        {
            UIProgressPercent.text = (int)(progressCircle.fillAmount * 100) + "%";
        }
    }

    public void UpdateMaxEnemyUI()
    {
        UIEnemyEliminations.text = "" + EnemyEliminations + "/" + MaxEnemyEliminations;
    }

    void GameOverCheck()
    {
        if(IsPlayerDetected)
        {
            if (playAlarmSound == true)
            {
                playAlarmSound = false;
                audiosource.PlayOneShot(AlarmSound);
            }

            waitGameOver += Time.deltaTime;
            if (waitGameOver > 1f)
            {
                GameOverReason = FailDetectionLevel;
                SceneManager.LoadScene(2);
            }
        }
        else if(EnemyEliminations > MaxEnemyEliminations)
        {
            if (playAlarmSound == true)
            {
                playAlarmSound = false;
                audiosource.PlayOneShot(AlarmSound);
            }

            waitGameOver += Time.deltaTime;
            if (waitGameOver > 1f)
            {
                GameOverReason = FailMaxEnemys;
                SceneManager.LoadScene(2);
            }
        }
    }

    void GameWinCheck()
    {
        if (progressCircle.fillAmount == 1f)
        {
            GameWon = true;
            waitWin += Time.deltaTime;
            if(waitWin > 2f)
            {
                if (TimePast < (float)Difficulty.Hard)
                {
                    DifficultyLevel = 4;
                }
                else if (TimePast < (float)Difficulty.Medium)
                {
                    DifficultyLevel = 3;
                }
                else if (TimePast < (float)Difficulty.Normal)
                {
                    DifficultyLevel = 2;
                }
                else if (TimePast > (float)Difficulty.Normal)
                {
                    DifficultyLevel = 1;
                }
                SceneManager.LoadScene(3);
            }
        }
    }

    void UpdateGameTime()
    {
        if ((TimePast / 60) < 10)
        {
            Minutes = "0" + ((int)TimePast / 60) + " : ";
        }
        else
        {
            Minutes = ((int)TimePast / 60) + " : ";
        }
        if ((TimePast % 60) < 10)
        {
            Seconds = "0" + ((int)TimePast % 60);
        }
        else
        {
            Seconds = ((int)TimePast % 60) +"";
        }
        UITimePast.text = Minutes + Seconds;
    }

}
