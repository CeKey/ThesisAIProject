using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Weapon : MonoBehaviour
{

    public AudioClip ShotAudio, EmptyShot;
    public float Distance;
    RaycastHit hitInfo;
    public int Bullets;

    public float MinShootTime;
    float lastShotTime;

    bool isRecoiling;
    Vector3 oldCamRotation;

    public GameObject BulletShell;
    public Transform ShellEjectPos;
    
    public Camera camera;
    AudioSource audiosource;
    Animator animator;
    public Animator weaponAnimator;
    Player playerScript;

    IngameMenu ingameMenu;

    // Use this for initialization
    void Start()
    {
        audiosource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        ingameMenu = GameObject.FindGameObjectWithTag("Canvas").GetComponent<IngameMenu>();
        playerScript = camera.gameObject.GetComponent<Player>();
        isRecoiling = false;

    }

    // Update is called once per frame
    void Update()
    {
        if(PlayerController.isWalking == false)
        {
            if (Input.GetButton("Fire1") && (Time.time - lastShotTime) >= MinShootTime && Bullets > 0 && ingameMenu.isMenuOpen == false)
            {
                if (Physics.Raycast(camera.transform.position, camera.transform.forward * Distance, out hitInfo))
                {
                    animator.SetBool("isShooting", true);
                    weaponAnimator.Play("Shoot");
                    playerScript.WeaponNoise = true;
                    isRecoiling = true;

                    Debug.DrawRay(camera.transform.position, camera.transform.forward * Distance, Color.green);
                    if (hitInfo.collider.tag == "Enemy")
                    {
                        hitInfo.collider.gameObject.GetComponent<EnemyAI>().ReceiveDamage(50.0f);
                    }
                    ImpactObject();
                    ShellEject();
                    HeadShotCondition();
                    audiosource.PlayOneShot(ShotAudio);
                    Bullets--;
                    lastShotTime = Time.time;
                }
            }
            else if (Input.GetButton("Fire1") && (Time.time - lastShotTime) >= MinShootTime && Bullets == 0)
            {
                lastShotTime = Time.time;
                isRecoiling = false;
                audiosource.PlayOneShot(EmptyShot);
                animator.SetBool("isShooting", false);
            }
            else if (Input.GetButtonUp("Fire1"))
            {
                isRecoiling = false;
                animator.SetBool("isShooting", false);
            }
        }
        else
        {
            isRecoiling = false;
            animator.SetBool("isShooting", false);
        }


        if (isRecoiling)
        {
            Recoil();
        }

    }

    private void FixedUpdate()
    {
    }

    void HeadShotCondition()
    {
        if(hitInfo.transform.tag == "EnemyHead")
        {
            hitInfo.transform.gameObject.GetComponentInParent<EnemyAI>().OnHeadshot();
        }
    }

    void Recoil()
    {
        oldCamRotation = camera.transform.localEulerAngles;
        oldCamRotation.x = Random.Range(-20f, -70f);
        oldCamRotation.y = Random.Range(15f, -15f);
        camera.transform.Rotate(oldCamRotation, 40f * Time.deltaTime);

        //camera.transform.localRotation = Quaternion.Slerp(camera.transform.localRotation, Quaternion.Euler(oldCamRotation), 4 * Time.deltaTime);
    }

    void ShellEject()
    {
        GameObject ShellInstance = Instantiate(BulletShell, ShellEjectPos.position, Quaternion.identity);
        ShellInstance.GetComponent<Rigidbody>().AddForce(transform.up * 100);
        ShellInstance.GetComponent<Rigidbody>().AddForce(transform.right*150);
        Destroy(ShellInstance, 1f);
    }

    void ImpactObject()
    {
        if(hitInfo.collider.gameObject.tag == "Wall")
        {
            playerScript.ImpactNoise = true;
            playerScript.BulletImpactLocation = hitInfo.point;
        }
    }

}
