using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;

public class SplineWalker : MonoBehaviour
{
	public BezierSpline spline;
	public float duration;

	public bool lookForward;
	private float progress;
	private float t;
    private Vector3 epsilonVector;

    private new CameraController camera;
    private int indexStopPoint;
    public List<float> stopPoints;

     void Start()
     {
        progress = 0.0f;
        indexStopPoint = 0;
        camera = GetComponent<CameraController>();
        epsilonVector = new Vector3(Single.Epsilon, Single.Epsilon, Single.Epsilon);
        if(spline != null)
            stopPoints = spline.stopPoints;
    }

    public IEnumerator StartPhase()
    {
        if (lookForward)
            camera.isActive = false;

        float duration = 1.0f;
        for (t = 0.0f; t < duration; t += Time.deltaTime)
        {
            Quaternion q = Quaternion.LookRotation(spline.GetDirection(progress));
            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(q.eulerAngles.x, q.eulerAngles.y, 0.0f), t/ duration);

            yield return null;
        }
    }

    public IEnumerator PlayUpdate ()
    {
        float dist = 1.0f;
        while (dist > 0.01f && progress != 1f)
        {
            progress += Time.deltaTime / duration;
            if (progress > 1f)
                progress = 1f;
            Vector3 position = spline.GetPoint(progress);
            transform.localPosition = position;
            dist = Vector3.Distance(position, spline.GetPoint(spline.stopPoints[indexStopPoint]));
            if (lookForward)
            {
                Quaternion q = Quaternion.LookRotation(spline.GetDirection(progress));
                transform.localRotation = Quaternion.Euler(q.eulerAngles.x, q.eulerAngles.y, 0.0f);
            }
            yield return null;
        }
        camera.isActive = true;
        camera.lookDirection = new Vector3(transform.localEulerAngles.y, transform.localEulerAngles.x, 0.0f);
        indexStopPoint++;
    }

}