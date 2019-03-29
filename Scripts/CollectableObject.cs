using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableObject : MonoBehaviour {

    AudioSource audiosource;
    public AudioClip collectsound;

	// Use this for initialization
	void Start () {
        audiosource = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            collider.gameObject.GetComponentInChildren<Player>().ObjectCollected = true;
            audiosource.PlayOneShot(collectsound);
            Destroy(gameObject, 0.4f);
        }
    }
}
