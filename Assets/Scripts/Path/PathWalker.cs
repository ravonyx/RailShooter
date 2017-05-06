using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;
using UnityEngine.VR;
using VRStandardAssets.Common;

public class PathWalker : MonoBehaviour
{
    public Path path;
    public float speed;
    public Transform pathTransform;
    [HideInInspector]
    public List<float> stopPoints;

    private new CameraController camera;
    private Vector3 epsilonVector;
    private int indexStopPoint = 0;
    private float progress = 1.0f;
    private float dist = 1.0f;
    private bool m_Walking;

    void Start()
    {
        camera = GetComponent<CameraController>();
        epsilonVector = new Vector3(Single.Epsilon, Single.Epsilon, Single.Epsilon);
        if (path != null)
            stopPoints = path.stopPoints;
    }

    public void Reset()
    {
        indexStopPoint = 0;
        progress = 1.0f;
    }

  /*  public IEnumerator StartPhase()
    {
        if (SessionData.GetGameType() == SessionData.GameType.SERIOUSSHOOTER)
        {
            camera.isActive = false;
            float duration = 1.0f;
            float t;
            for (t = 0.0f; t < duration; t += Time.deltaTime)
            {
                Quaternion q = Quaternion.LookRotation(path.GetPathPoint(progress).forward);
                transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(0.0f, q.eulerAngles.y, 0.0f), t / duration);

                yield return null;
            }
        }
    }*/

    void Update()
    {
        if (!m_Walking)
            return;

        progress += Time.deltaTime * speed / path.totalDistance;
        progress = Mathf.Clamp(progress, 0, path.totalDistance);

        Vector3 position = path.GetPathPoint(progress).point;
        Vector3 transformedPos = pathTransform.TransformPoint(position);
        transform.position = new Vector3(transformedPos.x, transformedPos.y + 1.5f, transformedPos.z);
        dist = Vector3.Distance(position, path.GetPathPoint(path.stopPoints[indexStopPoint]).point);
    }

    public IEnumerator PlayUpdate()
    {
        m_Walking = true;
        dist = 1.0f;
        while (dist > 0.1f && progress < path.totalDistance)
        {
            yield return null;
            // Quaternion q = Quaternion.LookRotation(path.GetPathPoint(progress).forward);
            // transform.localRotation = Quaternion.Euler(0.0f, q.eulerAngles.y, 0.0f);
        }
        m_Walking = false;
        //camera.Reset();
        indexStopPoint++;
    }
}