using UnityEngine;
using System.Collections.Generic;

public class SplineWalker : MonoBehaviour
{

	public BezierSpline spline;
	public float duration;
	public bool lookForward;
	private float progress;

    public List<float> stopPoints;
    void Start()
    {
        stopPoints = spline.stopPoints;
    }

	private void Update ()
    {
		progress += Time.deltaTime / duration;
		if (progress > 1f) 
				progress = 1f;

		Vector3 position = spline.GetPoint(progress);

        Debug.Log(position);
        Debug.Log(spline.GetPoint(stopPoints[0]));
        if (position == spline.GetPoint(stopPoints[0]))
        {
            Debug.Log("Stop");
        }
		transform.localPosition = position;
		if (lookForward)
			transform.LookAt(position + spline.GetDirection(progress));
	}
}