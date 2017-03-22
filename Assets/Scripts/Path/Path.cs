using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class Path : MonoBehaviour
{
    [HideInInspector]
    public float totalDistance;
    [HideInInspector]
    public List<float> stopPoints = new List<float>();

    private Vector3[] _points;
    private Vector3[] _upDirections;
    private float[] _distances;
    private int _numPoints;

    void Awake()
    {
		Update_Path();
	}
			
	public void Update_Path()
    {
		Transform[] children = new Transform[transform.childCount];
		Vector3[]   points = new Vector3[children.Length];
		Vector3[]   ups    = new Vector3[children.Length];

		for(int i=0; i< transform.childCount; i++)
        {
			children[i] = transform.GetChild(i);
			children[i].gameObject.name = "point " + i;

			points[i] = children[i].localPosition;
			ups[i] = transform.InverseTransformDirection(children[i].up);
		}

		if (transform.childCount > 1)
        {
			SetPoints(points, ups);
		}
	}

    public void SetPoints(Vector3[] points, Vector3[] ups)
    {
        _numPoints = points.Length;

        _points = points;
        _upDirections = ups;

        totalDistance = 0.0f;
        _distances = new float[_numPoints];
        for (int i = 0; i < _numPoints - 1; ++i)
        {
            _distances[i] = totalDistance;
            totalDistance += Vector3.Distance(_points[i], _points[i + 1]);
        }

        _upDirections[_numPoints - 1] = ups[_numPoints - 1];
        _distances[_distances.Length - 1] = totalDistance;
    }

    public PointPath GetPathPoint(float dist)
    {
        dist = Mathf.Clamp(dist, 0.0f, totalDistance);

        int[] _four_indices = new int[] { 0, 1, 2, 3 };
        Vector3[] _four_points = new Vector3[] { Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero };

        // find segment index
        int index = 1;
        if (_distances.Length > 0)
        {
            while (_distances[index] < dist)
            {
                index++;
            }
            // the segment in the middle
            float _interpolation = Mathf.InverseLerp(_distances[index - 1], _distances[index], dist);
            index = index % _numPoints;

            _four_indices[0] = Mathf.Clamp(index - 2, 0, _numPoints - 1);
            _four_indices[1] = ((index - 1) + _numPoints) % _numPoints;
            _four_indices[2] = index % _numPoints;
            _four_indices[3] = Mathf.Clamp(index + 1, 0, _numPoints - 1);

            // assign the four points with the segment in the middle
            _four_points[0] = _points[_four_indices[0]];
            _four_points[1] = _points[_four_indices[1]];
            _four_points[2] = _points[_four_indices[2]];
            _four_points[3] = _points[_four_indices[3]];


            PointPath _pathPoint = new PointPath();
            _pathPoint.point = CatmullRom(_four_points[0], _four_points[1], _four_points[2], _four_points[3], _interpolation);
            _pathPoint.forward = CatmullRom(_four_points[0], _four_points[1], _four_points[2], _four_points[3], _interpolation + 0.01f) - _pathPoint.point;
            _pathPoint.forward.Normalize();

            // 90 degree turn to right
            Vector3 lerpBetweentwo = Vector3.Lerp(_upDirections[_four_indices[1]], _upDirections[_four_indices[2]], _interpolation);
            _pathPoint.right = Vector3.Cross(lerpBetweentwo, _pathPoint.forward).normalized;
            // 90 degree turn to up
            _pathPoint.up = Vector3.Cross(_pathPoint.forward, _pathPoint.right).normalized;

            return _pathPoint;
        }
        return new PointPath();
    }

    public static Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        //The coefficients of the cubic polynomial (except the 0.5f * which I added later for performance)
        Vector3 a = 2f * p1;
        Vector3 b = p2 - p0;
        Vector3 c = 2f * p0 - 5f * p1 + 4f * p2 - p3;
        Vector3 d = -p0 + 3f * p1 - 3f * p2 + p3;

        //The cubic polynomial: a + b * t + c * t^2 + d * t^3
        Vector3 pos = 0.5f * (a + (b * t) + (c * t * t) + (d * t * t * t));
        return pos;
    }

    public void AddStopPoint(float index)
    {
        if (!stopPoints.Contains(index))
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
    public void Reset()
    {
        stopPoints = new List<float>();
    }

    #region Gizmo

    private void OnDrawGizmos()
	{
		DrawGizmos(false);
	}

	private void OnDrawGizmosSelected()
	{
		DrawGizmos(true);
	}

	private void DrawGizmos(bool selected)
	{
		Update_Path();

		if(transform.childCount <= 1)
			return;

        PointPath prev = GetPathPoint(0.0f);
		float dist = -1.0f;
		while(dist < totalDistance)
        {
			dist = Mathf.Clamp(dist + 1.0f,0, totalDistance);

            PointPath next = GetPathPoint(dist);

			Gizmos.color = selected ? new Color(0, 1, 1, 1) : new Color(0, 1, 1, 0.5f);
			Gizmos.DrawLine(transform.TransformPoint(prev.point), transform.TransformPoint(next.point));

            //draw up vector
            Gizmos.color = selected ? Color.green : new Color(0, 1, 0, 0.5f);
			Gizmos.DrawLine(transform.TransformPoint(next.point), transform.TransformPoint(next.point) + transform.TransformDirection(next.up));

            //draw right vector
			Gizmos.color = selected ? Color.red : new Color(1, 0, 0, 0.5f);
			Gizmos.DrawLine(transform.TransformPoint(next.point), transform.TransformPoint(next.point) + transform.TransformDirection(next.right * 0.5f));

			prev = next;
		}
	}
    #endregion
}

public struct PointPath
{
	public Vector3 point;
	public Vector3 forward;
	public Vector3 up;
	public Vector3 right;

	public PointPath(Vector3 point, Vector3 forward, Vector3 up, Vector3 right)
	{
		this.point = point;
		this.forward = forward;
		this.up = up;
		this.right = right;
	}
}

