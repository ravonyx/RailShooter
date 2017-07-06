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

    bool pressed;
    int posTotal = 0;
    int posOk = 0;
    private float m_Z;
    public bool trainingRunning;

    private MeshFilter m_mesh;
    private Renderer m_renderer;

    [SerializeField]
    PKFxFX m_fx;
    [SerializeField]
    AudioClip m_failClip;
    [SerializeField]
    AudioClip m_goodClip;

    [SerializeField]
    PKFxFX m_fxCountDown;

    private AudioSource m_audioSource;

    void Start ()
    {
        m_audioSource = GetComponent<AudioSource>();
        m_mesh = GetComponent<MeshFilter>();
        m_renderer = GetComponent<Renderer>();

        m_Z = transform.localPosition.z + 5;
        transform.localPosition = new Vector3(XMin, YMin, m_Z);
        trainingRunning = true;
        pressed = false;

        StartCoroutine(Coundown());
    }

    IEnumerator Coundown()
    {
        PKFxManager.Sampler textAttr = m_fxCountDown.GetSampler("Text");
        yield return new WaitForSeconds(2.0f);

        StartCountDownEffect(5, textAttr);
        yield return new WaitForSeconds(1.1f);

        StartCountDownEffect(4, textAttr);
        yield return new WaitForSeconds(1.1f);

        StartCountDownEffect(3, textAttr);
        yield return new WaitForSeconds(1.1f);

        StartCountDownEffect(2, textAttr);
        yield return new WaitForSeconds(1.1f);

        StartCountDownEffect(1, textAttr);
        yield return new WaitForSeconds(1.1f);

        StartCountDownEffect(0, textAttr);
        yield return new WaitForSeconds(2.0f);

        StartCoroutine(targetPositionUpdate());
        eyemanagerScript.startEyeRecord(true);
    }

    void StartCountDownEffect(int count, PKFxManager.Sampler textAttr)
    {
        m_fxCountDown.StopEffect();

        int nb = count;
        if(count == 0)
            textAttr.m_Text = "Start!";
        else
            textAttr.m_Text = count.ToString();
        m_fxCountDown.StartEffect();
    }
    void Update ()
    {
        if (trainingRunning)
        {
            if (m_targetable)
                m_mesh.mesh = m_Mesh1;
            else
                m_mesh.mesh = m_Mesh2;

            if (Input.GetKeyDown(KeyCode.Space))
                pressed = true;
        }
        else
            ;// Debug.Log("training Over " + posOk + "Goodpress / " + posTotal);
    }

    IEnumerator targetPositionUpdate()
    {
        if (pressed && m_targetable)
        {
            m_fx.transform.position = transform.position;
            m_fx.StartEffect();
            m_audioSource.clip = m_goodClip;
            m_audioSource.Play();

            posOk++;
        }
        else if (pressed && !m_targetable)
        {
            m_audioSource.clip = m_failClip;
            m_audioSource.Play();
        }
        else if(!pressed && m_targetable)
        {
            m_audioSource.clip = m_failClip;
            m_audioSource.Play();
        }
        else if(!pressed && !m_targetable)
        {
            m_audioSource.clip = m_goodClip;
            m_audioSource.Play();

            posOk++;
        }

        float X = transform.localPosition.x;
        float Y = transform.localPosition.y;

        if (transform.localPosition.x < XMax)
            X += XIncrement;
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
