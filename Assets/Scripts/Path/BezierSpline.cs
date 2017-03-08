using UnityEngine;
using System;
using System.Collections.Generic;

public class BezierSpline : MonoBehaviour
{

	[SerializeField]
	private Vector3[] points;

    [SerializeField]
    public List<float> stopPoints;

	[SerializeField]
	private bool loop;

    public bool Loop {
		get {
			return loop;
		}
		set {
			loop = value;
			if (value == true) 
				SetControlPoint(0, points[0]);
			
		}
	}

    public int ControlPointCount {
		get {
			return points.Length;
		}
	}

	public Vector3 GetControlPoint (int index) {
		return points[index];
	}

	public void SetControlPoint (int index, Vector3 point)
    {
		if (index % 3 == 0) {
			Vector3 delta = point - points[index];
			if (loop) {
				if (index == 0) {
					points[1] += delta;
					points[points.Length - 2] += delta;
					points[points.Length - 1] = point;
				}
				else if (index == points.Length - 1) {
					points[0] = point;
					points[1] += delta;
					points[index - 1] += delta;
				}
				else {
					points[index - 1] += delta;
					points[index + 1] += delta;
				}
			}
			else {
				if (index > 0) {
					points[index - 1] += delta;
				}
				if (index + 1 < points.Length) {
					points[index + 1] += delta;
				}
			}
		}
		points[index] = point;
	}

	public int CurveCount
    {
		get
        {
			return (points.Length - 1) / 3;
		}
	}

	public Vector3 GetPoint (float t)
    {
		int i;
		if (t >= 1f)
        {
			t = 1f;
			i = points.Length - 4;
		}
		else
        {
			t = Mathf.Clamp01(t) * CurveCount;
			i = (int)t;
			t -= i;
			i *= 3;
		}
		return transform.TransformPoint(Bezier.GetPoint(points[i], points[i + 1], points[i + 2], points[i + 3], t));
	}
	
	public Vector3 GetVelocity (float t)
    {
		int i;
		if (t >= 1f)
        {
			t = 1f;
			i = points.Length - 4;
		}
		else
        {
			t = Mathf.Clamp01(t) * CurveCount;
			i = (int)t;
			t -= i;
			i *= 3;
		}
		return transform.TransformPoint(Bezier.GetFirstDerivative(points[i], points[i + 1], points[i + 2], points[i + 3], t)) - transform.position;
	}
	
	public Vector3 GetDirection (float t)
    {
		return GetVelocity(t).normalized;
	}

	public void AddCurve ()
    {
		Vector3 point = points[points.Length - 1];
        int oldSize = CurveCount;

        Array.Resize(ref points, points.Length + 3);
        int newSize = CurveCount;

        point.x += 1f;
		points[points.Length - 3] = point;
		point.x += 1f;
		points[points.Length - 2] = point;
		point.x += 1f;
		points[points.Length - 1] = point;

        for (int i = 0; i < stopPoints.Count; i++)
        {
            float ratio = (float)oldSize / (float)newSize;
            stopPoints[i] = stopPoints[i] * ratio;
        }

        if (loop) 
			points[points.Length - 1] = points[0];
	}

    public void AddStopPoint(float index)
    {
        if(!stopPoints.Contains(index))
        {
            stopPoints.Add(index);
        }
    }

    public void DeleteStopPoint(float index)
    {
        if (stopPoints.Contains(index))
        {
            stopPoints.Remove(index);
        }
    }

    public void Reset ()
    {
		points = new Vector3[] 
        {
			new Vector3(1f, 0f, 0f),
			new Vector3(2f, 0f, 0f),
			new Vector3(3f, 0f, 0f),
			new Vector3(4f, 0f, 0f)
		};

        stopPoints = new List<float>();
	}
}