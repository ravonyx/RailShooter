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

    private CameraController camera;

    public List<float> stopPoints;
    void Start()
    {
        camera = GetComponent<CameraController>();
        camera.isActive = (isWalking == true) ? false : true;
        epsilonVector = new Vector3(Single.Epsilon, Single.Epsilon, Single.Epsilon);
        stopPoints = spline.stopPoints;
    }

    private void Update ()
    {
        if(isWalking)
		    progress += Time.deltaTime / duration;
		if (progress > 1f) 
				progress = 1f;

		Vector3 position = spline.GetPoint(progress);
        float dist = Vector3.Distance(position, spline.GetPoint(spline.stopPoints[0]));
        if (dist <= 0.01f)
        {
            Debug.Log("look direction end walk " + camera.lookDirection);

            camera.isActive = true;
            isWalking = false;
        }
        else
        {
            camera.isActive = false;
            transform.localPosition = position;
            isWalking = true;
        }
        if (lookForward && isWalking)
        {
            camera.lookDirection += new Vector3(spline.GetDirection(progress).x, spline.GetDirection(progress).y, 0.0f);

            Quaternion q = Quaternion.LookRotation(spline.GetDirection(progress));
            Debug.Log(" look direction  " + camera.lookDirection);
            transform.localRotation = Quaternion.Euler(q.eulerAngles.x, q.eulerAngles.y, 0.0f);

            //transform.LookAt(position + spline.GetDirection(progress));
        }
        //transform.localRotation = Qua Quaternion.LookRotation(relativePos);
    }
}