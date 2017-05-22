using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;
using UnityEngine.VR;
using VRStandardAssets.Common;

public class PathWalker : MonoBehaviour
{
    [SerializeField]
    private Path m_path;
    [SerializeField]
    private float m_speed;
    [SerializeField]
    private Transform m_pathTransform;

    [HideInInspector]
    public List<float> StopPoints
    {   get
        {
            return m_stopPoints;
        }
    }
    private List<float> m_stopPoints;
    
    private CameraController m_camera;
    private Vector3 m_epsilonVector;
    private int m_indexStopPoint = 0;
    private float m_progress = 1.0f;
    private float m_dist = 1.0f;
    private bool m_walking;
    public bool Walking
    {
        get
        {
            return m_walking;
        }
        set
        {
            m_walking = value;
        }
    }

    void Start()
    {
        m_camera = GetComponent<CameraController>();
        m_epsilonVector = new Vector3(Single.Epsilon, Single.Epsilon, Single.Epsilon);
        if (m_path != null)
            m_stopPoints = m_path.stopPoints;
    }

    public void Reset()
    {
        m_indexStopPoint = 0;
        m_progress = 1.0f;
    }

    void Update()
    {
        if (!m_walking || m_indexStopPoint >= m_path.stopPoints.Count)
            return;

        Vector3 position = m_path.GetPathPoint(m_progress).point;
        Vector3 transformedPos = m_pathTransform.TransformPoint(position);
        transform.position = new Vector3(transformedPos.x, transformedPos.y + 1.5f, transformedPos.z);
    }

    public IEnumerator PlayUpdate()
    {
        m_walking = true;
        m_dist = 1.0f;

        while (m_dist > 0.5f && m_progress < m_path.totalDistance)
        {
            yield return null;

            if (m_walking)
            {
                m_progress += Time.deltaTime * m_speed / m_path.totalDistance;
                m_progress = Mathf.Clamp(m_progress, 0, m_path.totalDistance);
            }

            Vector3 position = m_path.GetPathPoint(m_progress).point;
            m_dist = Vector3.Distance(position, m_path.GetPathPoint(m_path.stopPoints[m_indexStopPoint]).point);
        }

        m_walking = false;
        m_indexStopPoint++;
    }
}