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

    [SerializeField]
    private Mesh m_Mesh1;
    [SerializeField]
    private Mesh m_Mesh2;

    bool pressed = true;
    int posTotal = 0;
    int posOk = 0;
    private float m_Z;
    public bool trainingRunning = false;

    private MeshFilter m_mesh;
    private Renderer m_renderer;

    void Start ()
    {
        m_Z = transform.localPosition.z + 5;
        transform.localPosition = new Vector3(XMin, YMin, m_Z);
        trainingRunning = true;
        StartCoroutine(targetPositionUpdate());
        eyemanagerScript.startEyeRecord(true);

        m_mesh = GetComponent<MeshFilter>();
        m_renderer = GetComponent<Renderer>();
    }

    private void FixedUpdate()
    {

    }

    void Update ()
    {
        if (trainingRunning)
        {
            if (m_targetable)
            {
                m_renderer.material.color = new Color(0, 0, 255);
                m_mesh.mesh = m_Mesh1;
            }
            else
            {
                m_renderer.material.color = new Color(255, 0, 0);
                m_mesh.mesh = m_Mesh2;
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                pressed = true;
            }
        }
        else
            ;// Debug.Log("training Over " + posOk + "Goodpress / " + posTotal);
    }

    IEnumerator targetPositionUpdate()
    {
        if (pressed && m_targetable)
        {
            Debug.Log("Good Job");
            posOk++;
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
            Debug.Log("training Over " + posOk + "Goodpress / " + posTotal);
        }
    }
}
