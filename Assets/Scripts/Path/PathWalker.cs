using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RailShooter.Assets;

public class PathWalker : MonoBehaviour
{
    [SerializeField]
    private RailShooterController m_railShooterController;
    [SerializeField]
    private Path m_path;
    [SerializeField]
    private float m_speed;
    [SerializeField]
    private float m_deltaY;

    [HideInInspector]
    public List<float> StopPoints
    {   get
        {
            return m_stopPoints;
        }
    }
    private List<float> m_stopPoints;

    private Transform m_pathTransform;
    private int m_indexStopPoint = 0;
    private float m_progress = 1.0f;
    private float m_dist = 1.0f;

    [SerializeField]
    private bool m_enemy;
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
        if(m_enemy)
            m_walking = true;
        else
            m_walking = false;

        m_pathTransform = m_path.GetComponent<Transform>();
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
        if (!m_walking || m_indexStopPoint >= m_path.stopPoints.Count || !m_railShooterController.IsPlaying)
            return;

        Vector3 position = m_path.GetPathPoint(m_progress).point;
        Vector3 transformedPos = m_pathTransform.TransformPoint(position);
        transform.rotation = Quaternion.LookRotation(m_path.GetPathPoint(m_progress).forward);

        transform.position = new Vector3(transformedPos.x, transformedPos.y + m_deltaY, transformedPos.z);
        if (m_enemy)
            transform.rotation = Quaternion.LookRotation(m_path.GetPathPoint(m_progress).forward);
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

    public IEnumerator PlayUpdateBackward()
    {
        m_progress = m_path.totalDistance;
        m_walking = true;
        m_enemy = true;
        m_dist = 1.0f;

        while (m_progress > 1.0)
        {
            yield return null;

            if (m_walking && m_railShooterController.IsPlaying)
            {
                m_progress -= Time.deltaTime * m_speed / m_path.totalDistance;
                m_progress = Mathf.Clamp(m_progress, 0, m_path.totalDistance);
            }
        }

        m_walking = false;
    }
}