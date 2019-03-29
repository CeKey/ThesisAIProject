using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelObjects : MonoBehaviour {

    public Transform[] StartPoints;
    public Transform[] MarkPoints;

    Transform StartPoint;
    Transform MarkPoint;

    public GameObject Player;
    GameObject playerRef;
    Player playerScript;

    public GameObject GrassContainer;

	// Use this for initialization
	void Start () {
        
        StartPoint = GetRandomStartPoint();
        MarkPoint = GetRandomMarkPoint();

        playerRef = Instantiate(Player, StartPoint.position, Quaternion.identity);
        playerScript = playerRef.GetComponentInChildren<Player>();
        playerScript.MarkPoint = MarkPoint;

        if(QualitySettings.GetQualityLevel() < 3)
        {
            GrassContainer.SetActive(false);
        }
        else
        {
            GrassContainer.SetActive(true);
        }
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void FixedUpdate()
    {
        
    }

    Transform GetRandomStartPoint()
    {
        return StartPoints[Random.Range(0, StartPoints.Length)];
    }

    Transform GetRandomMarkPoint()
    {
        int tmpRnd = Random.Range(0, MarkPoints.Length);
        for(int i = 0; i < MarkPoints.Length; i++)
        {
            if(tmpRnd != i)
            {
                MarkPoints[i].gameObject.SetActive(false);
            }
			else if(tmpRnd == i)
			{
				MarkPoints[i].gameObject.SetActive(true);
			}
        }
        return MarkPoints[tmpRnd];
    }
}
