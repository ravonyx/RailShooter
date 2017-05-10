using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetMove : MonoBehaviour {

    [SerializeField]
    float XMin;
    [SerializeField]
    float XMax;
    [SerializeField]
    float YMin;
    [SerializeField]
    float Ymax;
    [SerializeField]
    float XIncrement;
    [SerializeField]
    float YIncrement;
    [SerializeField]
    float tick;
    [SerializeField]
    Eyemanager eyemanagerScript;
    bool m_targetable = false;

    bool pressed = true;
    int posTotal = 0;
    int posOk = 0;
    private float m_Z;
    public bool trainingRunning = false;
	// Use this for initialization
	void Start () {
        m_Z = transform.localPosition.z + 5;
        transform.localPosition = new Vector3(XMin, YMin, m_Z);
        trainingRunning = true;
        StartCoroutine(targetPositionUpdate());
        eyemanagerScript.startEyeRecord(true);
	}

    private void FixedUpdate()
    {

    }
    // Update is called once per frame
    void Update () {
       
        if (trainingRunning)
        {
            if (m_targetable)
            {
                this.GetComponent<Renderer>().material.color = new Color(0, 0, 255);
            }
            else
                this.GetComponent<Renderer>().material.color = new Color(255, 0, 0);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                pressed = true;
            }
                      
        }
        else
            Debug.Log("training Over " + posOk + "Goodpress / " + posTotal);
    }

    IEnumerator targetPositionUpdate()
    {
        if (pressed && m_targetable)
        {
            Debug.Log("Good Job");
           // posOk++;
        }
        else if (pressed && !m_targetable)
        {
            ;// Debug.Log("fail");
        }
        else if(!pressed && m_targetable)
        {
            ;// Debug.Log("fail");
        }
        else if(!pressed && !m_targetable)
        {
           // Debug.Log("GoodJob");
            posOk++;
        }


        float X = transform.localPosition.x;
        float Y = transform.localPosition.y;

        if (transform.localPosition.x < XMax)
        {
            X += XIncrement;
            //Debug.Log("Increment");
        }
        else
        {
            X = XMin;
            Y -= YIncrement;
        }
        transform.localPosition = new Vector3(X, Y, m_Z);
        if (Random.Range(0, 2) == 0)
            m_targetable = false;
        else
            m_targetable = true;
        posTotal++;
        pressed = false;
        yield return new WaitForSeconds(tick);

        if (Y > Ymax)
            StartCoroutine(targetPositionUpdate());
        else
        {
            trainingRunning = false;
            eyemanagerScript.startEyeRecord(false);
        }
    }

    
}
