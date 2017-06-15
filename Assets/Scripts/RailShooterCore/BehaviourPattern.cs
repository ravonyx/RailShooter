using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourPattern : MonoBehaviour
{
    public enum BehaviourType
    {
        TRANSLATE,
        SCALE,
        ROTATE,
    };

    [SerializeField]
    private BehaviourType m_behaviourType;

    [SerializeField]
    private bool m_rotateX;
    [SerializeField]
    private bool m_rotateY;
    [SerializeField]
    private bool m_rotateZ;

    private Vector3 m_originPoint;
    [SerializeField]
    private Vector3 m_dstPoint;

    [SerializeField]
    float m_speedTranslate;
    [SerializeField]
    float m_speedRotate;

    [SerializeField]
    private float m_initScale;
    [SerializeField]
    private float m_targetScale;

    private bool m_upScale;
    private float m_currentScale;

    private float m_deltaTime = 2 / 100;
    private float m_dx;

    void Start ()
    {
        m_originPoint = transform.position;
        m_currentScale = m_initScale;
        m_dx = (m_targetScale - m_initScale) / 100;
        if (m_behaviourType == BehaviourType.SCALE)
            StartCoroutine(Scale());
        if (m_behaviourType == BehaviourType.ROTATE)
            StartCoroutine(Rotate());
        if (m_behaviourType == BehaviourType.TRANSLATE)
            StartCoroutine(Translate());
    }

    private IEnumerator Scale()
    {
        while (true)
        {
            while (m_upScale)
            {
                m_currentScale += m_dx;
                if (m_currentScale > m_targetScale)
                {
                    m_upScale = false;
                    m_currentScale = m_targetScale;
                }
                transform.localScale = Vector3.one * m_currentScale;
                yield return new WaitForSeconds(m_deltaTime);
            }

            while (!m_upScale)
            {
                m_currentScale -= m_dx;
                if (m_currentScale < m_initScale)
                {
                    m_upScale = true;
                    m_currentScale = m_initScale;
                }
                transform.localScale = Vector3.one * m_currentScale;
                yield return new WaitForSeconds(m_deltaTime);
            }
        }
    }

    IEnumerator Rotate()
    {
        while (true)
        {
            if (m_rotateX)
                transform.Rotate(m_speedRotate * Time.deltaTime, 0, 0);
            if (m_rotateY)
                transform.Rotate(0, m_speedRotate * Time.deltaTime, 0);
            if (m_rotateZ)
                transform.Rotate(0, 0, m_speedRotate * Time.deltaTime);
            yield return new WaitForSeconds(m_deltaTime);
        }
    }


    IEnumerator Translate()
    {
        while (true)
        {
            var i = Mathf.PingPong(Time.time * m_speedTranslate, 1);
            transform.position = Vector3.Lerp(m_originPoint, m_dstPoint, i);
            yield return new WaitForSeconds(m_deltaTime);
        }
    }

}
