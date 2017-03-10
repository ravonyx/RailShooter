using UnityEngine;
using System.Collections.Generic;
using System;

public class SplineWalker : MonoBehaviour
{
	public BezierSpline spline;
	public float duration;
	public bool lookForward;
	private float progress;
    private Vector3 epsilonVector;
    public bool isWalking;

    private new CameraController camera;
    private int indexStopPoint;

    public List<float> stopPoints;
    void Start()
    {
        indexStopPoint = 0;
        camera = GetComponent<CameraController>();
        epsilonVector = new Vector3(Single.Epsilon, Single.Epsilon, Single.Epsilon);
        stopPoints = spline.stopPoints;
    }

    private void Update ()
    {
        camera.isActive = (isWalking == true) ? false : true;
        if (isWalking)
        {
            progress += Time.deltaTime / duration;
            if (progress > 1f)
                progress = 1f;

            Vector3 position = spline.GetPoint(progress);
            float dist = Vector3.Distance(position, spline.GetPoint(spline.stopPoints[indexStopPoint]));
            if (dist <= 0.01f || progress == 1f)
            {
                isWalking = false;
                indexStopPoint++;
            }
            else
                transform.localPosition = position;

            if (lookForward)
            {
                camera.lookDirection += new Vector3(spline.GetDirection(progress).x, spline.GetDirection(progress).y, 0.0f);
                Quaternion q = Quaternion.LookRotation(spline.GetDirection(progress));
                transform.localRotation = Quaternion.Euler(q.eulerAngles.x, q.eulerAngles.y, 0.0f);
            }
        }
        if (Input.GetKeyDown(KeyCode.Space))
            isWalking = true;
    }
}