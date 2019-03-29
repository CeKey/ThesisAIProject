using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneVisuals : MonoBehaviour {

    public float PropellerTorque;
    GameObject[] Propellers;

    float waitMovement;
    public float UpDownSpeed;

	// Use this for initialization
	void Start () {
        waitMovement = 0f;
        Propellers = GameObject.FindGameObjectsWithTag("Propeller");
	}
	
	// Update is called once per frame
	void Update () {
        waitMovement += Time.deltaTime;
        HoverSimulation();
	}

    private void FixedUpdate()
    {
        PropTorque();
    }

    void HoverSimulation()
    {
        if (waitMovement <= 1f)
        {
            transform.Translate(new Vector3(0f, UpDownSpeed * Time.deltaTime, 0f));
        }
        else if (waitMovement > 1f && waitMovement < 2f)
        {
            transform.Translate(new Vector3(0f, -UpDownSpeed * Time.deltaTime, 0f));
        }
        else if (waitMovement > 2f)
        {
            waitMovement = 0f;
        }
    }

    void PropTorque()
    {
        for(int i = 0; i<Propellers.Length; i++)
        {
            Propellers[i].transform.Rotate(Vector3.forward * PropellerTorque * Time.deltaTime);
        }
    }
}
