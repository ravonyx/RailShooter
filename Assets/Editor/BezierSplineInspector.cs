using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BezierSpline))]
public class BezierSplineInspector : Editor
{

	private const int stepsPerCurve = 10;
	private const float directionScale = 0.5f;
	private const float handleSize = 0.05f;
	private const float pickSize = 0.1f;

	private static Color[] modeColors = 
    {
		Color.white,
		Color.yellow,
		Color.cyan
	};

	private BezierSpline spline;
	private Transform handleTransform;
	private Quaternion handleRotation;

	private int selectedIndexCtrlPoint = -1;
	private float selectedIndexStopPoint = -1;

    private float positionToDelete = -1.0f;

    bool managePoints = false;
    public override void OnInspectorGUI ()
    {
		spline = target as BezierSpline;
		EditorGUI.BeginChangeCheck();

        bool loop = EditorGUILayout.Toggle("Loop", spline.Loop);
        if (EditorGUI.EndChangeCheck())
        {
			Undo.RecordObject(spline, "Toggle Loop");
			EditorUtility.SetDirty(spline);
			spline.Loop = loop;
		}

        if(managePoints = GUILayout.Toggle(managePoints, "Manage points"))
            Repaint();
       
        if(managePoints)
        {
            if (positionToDelete != -1.0f)
            {
                if (GUILayout.Button("Delete Point"))
                {
                    Undo.RecordObject(spline, "Delete Point");
                    spline.DeleteStopPoint(positionToDelete);
                    EditorUtility.SetDirty(spline);
                    positionToDelete = -1.0f;
                }
            }
            GUILayout.Label("List of all stop points");
            for (int i = 0; i < spline.stopPoints.Count; i++)
            {
                EditorGUILayout.Vector3Field("Position", spline.GetPoint(spline.stopPoints[i]));
            }

            if (selectedIndexStopPoint >= 0)
                DrawStopPointInspector();
        }

        else
        {
            if (selectedIndexCtrlPoint >= 0 && selectedIndexCtrlPoint < spline.ControlPointCount)
                DrawSelectedPointInspector();

            if (GUILayout.Button("Add Curve"))
            {
                Undo.RecordObject(spline, "Add Curve");
                spline.AddCurve();
                EditorUtility.SetDirty(spline);
            }
        }
    }

    private void DrawStopPointInspector()
    {
        GUILayout.Label("Stop Point");
        EditorGUI.BeginChangeCheck();
        Vector3 point = EditorGUILayout.Vector3Field("Position", spline.GetPoint(selectedIndexStopPoint));
        if (GUILayout.Button("Add To Stop Point"))
        {
            Undo.RecordObject(spline, "Add Stop Point");
            spline.AddStopPoint(selectedIndexStopPoint);
            EditorUtility.SetDirty(spline);
        }
    }

    private void DrawSelectedPointInspector()
    {
		GUILayout.Label("Selected Point");
		EditorGUI.BeginChangeCheck();
		Vector3 point = EditorGUILayout.Vector3Field("Position", spline.GetControlPoint(selectedIndexCtrlPoint));
		if (EditorGUI.EndChangeCheck())
        {
			Undo.RecordObject(spline, "Move Point");
			EditorUtility.SetDirty(spline);
			spline.SetControlPoint(selectedIndexCtrlPoint, point);
		}
	}

	private void OnSceneGUI ()
    {
		spline = target as BezierSpline;
		handleTransform = spline.transform;
		handleRotation = Tools.pivotRotation == PivotRotation.Local ? handleTransform.rotation : Quaternion.identity;
		
        //Draw controls points with Bezier
        if(!managePoints)
        {
            Vector3 p0 = DrawControlPoint(0);
            for (int i = 1; i < spline.ControlPointCount; i += 3)
            {
                Vector3 p1 = DrawControlPoint(i);
                Vector3 p2 = DrawControlPoint(i + 1);
                Vector3 p3 = DrawControlPoint(i + 2);

                Handles.color = Color.gray;
                Handles.DrawLine(p0, p1);
                Handles.DrawLine(p2, p3);

                Handles.DrawBezier(p0, p3, p1, p2, Color.white, null, 2f);
                p0 = p3;
            }
        }
		
		ShowDirections();
        DrawPointsAndStopPoints();
    }

	private void ShowDirections()
    {
		Handles.color = Color.red;
		Vector3 point = spline.GetPoint(0f);
		Handles.DrawLine(point, point + spline.GetDirection(0f) * directionScale);
		int steps = stepsPerCurve * spline.CurveCount;
		for (int i = 1; i <= steps; i++)
        {
			point = spline.GetPoint(i / (float)steps);
            Handles.color = Color.red;
            Handles.DrawLine(point, point + spline.GetDirection(i / (float)steps) * directionScale);
        }
	}

    private void DrawPointsAndStopPoints()
    {
            //Draw all points on curve
            Handles.color = Color.gray;
            Vector3 point = spline.GetPoint(0f);
            int steps = stepsPerCurve * spline.CurveCount;

            for (int i = 1; i <= steps; i++)
            {
                point = spline.GetPoint(i / (float)steps);
                if (Handles.Button(point, handleRotation, handleSize, pickSize, Handles.SphereCap))
                {
                    selectedIndexStopPoint = i / (float)steps;
                    positionToDelete = -1.0f;
                    Repaint();
                }
            }

        //Draw all stop points in blue
        Handles.color = Color.blue;
        for (int i = 0; i < spline.stopPoints.Count; i++)
        {
            if (Handles.Button(spline.GetPoint(spline.stopPoints[i]), handleRotation, handleSize, pickSize, Handles.DotCap))
            {
                positionToDelete = spline.stopPoints[i];
                Repaint();
            }
        }
    }

    private Vector3 DrawControlPoint (int index)
    {
		Vector3 controlPoint = handleTransform.TransformPoint(spline.GetControlPoint(index));

        Handles.color = Color.gray;
		if (Handles.Button(controlPoint, handleRotation, handleSize, pickSize, Handles.DotCap))
        {
            selectedIndexCtrlPoint = index;
            positionToDelete = -1;
            Repaint();
        }
		if (selectedIndexCtrlPoint == index)
        {
			EditorGUI.BeginChangeCheck();
            controlPoint = Handles.DoPositionHandle(controlPoint, handleRotation);
			if (EditorGUI.EndChangeCheck())
            {
				Undo.RecordObject(spline, "Move Point");
				EditorUtility.SetDirty(spline);
				spline.SetControlPoint(index, handleTransform.InverseTransformPoint(controlPoint));
			}
		}
		return controlPoint;
	}
}