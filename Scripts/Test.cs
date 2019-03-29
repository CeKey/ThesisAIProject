using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Test : MonoBehaviour {
    public GameObject Waypoint;
    public float Speed;
    public float Pursuit;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 velocity = (Waypoint.transform.position - transform.position).normalized * (Speed * Time.deltaTime);
        transform.position += velocity*Pursuit;
	}
}
