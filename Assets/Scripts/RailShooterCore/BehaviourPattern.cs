using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourPattern : MonoBehaviour
{

    public enum BehaviourType
    {
        TRANSLATEY,
        TRANSLATEX,
        ROTATE,
        ROTATETOWARDS,
    };

    [SerializeField]
    private BehaviourType m_BehaviourType;

    float m_yCenter;
    float m_xCenter;

    [SerializeField]
    float m_yAmplitude;
    [SerializeField]
    float m_xAmplitude;
    [SerializeField]
    float m_speedTranslate;
    [SerializeField]
    float m_speedRotate;

    void Start ()
    {
        m_yCenter = transform.position.y;
        m_xCenter = transform.position.x;
    }
	
	void Update ()
    {
        if(m_BehaviourType == BehaviourType.ROTATE)
            transform.Rotate(0, m_speedRotate * Time.deltaTime, 0);
        else if(m_BehaviourType == BehaviourType.TRANSLATEY)
            transform.position = new Vector3(transform.position.x, m_yCenter + Mathf.PingPong(Time.time * m_speedTranslate, m_yAmplitude) - m_yAmplitude / 2f, transform.position.z);
        else if (m_BehaviourType == BehaviourType.TRANSLATEX)
            transform.position = new Vector3(m_xCenter + Mathf.PingPong(Time.time * m_speedTranslate, m_xAmplitude) - m_xAmplitude / 2f, transform.position.y, transform.position.z);
    }
}
