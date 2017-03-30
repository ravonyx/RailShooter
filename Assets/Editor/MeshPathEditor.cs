using UnityEngine;
using System.Collections;
using UnityEditor;
	
[CustomEditor(typeof(MeshPath))]
[CanEditMultipleObjects]
public class MeshPathEditor : Editor
{
    MeshPath meshPath;
    Path path;
    private float selectedIndexStopPoint = -1.0f;
    private float positionToDelete = -1.0f;

    public override void OnInspectorGUI()
    {
        meshPath = target as MeshPath;
        DrawDefaultInspector();

        if (GUILayout.Button("Shape It"))
            meshPath.ShapeIt();

        path = meshPath.path;
        if(path)
        {
            if (path.stopPoints.Count > 0)
                GUILayout.Label("List of all stop points");
            for (int i = 0; i < path.stopPoints.Count; i++)
            {
                EditorGUILayout.Vector3Field("Position", path.GetPathPoint(path.stopPoints[i]).point);
            }
            if (selectedIndexStopPoint >= 0)
            {
                if (GUILayout.Button("Add To Stop Point"))
                {
                    path.AddStopPoint(selectedIndexStopPoint);
                    EditorUtility.SetDirty(path);
                }
            }
            if (positionToDelete != -1.0f)
            {
                if (GUILayout.Button("Delete Point"))
                {
                    path.DeleteStopPoint(positionToDelete);
                    EditorUtility.SetDirty(path);
                    positionToDelete = -1.0f;
                }
            }
        }
    }

    private void OnSceneGUI()
    {
        meshPath = target as MeshPath;
        Transform transform = meshPath.transform;

        //Draw all points on curve
        Handles.color = Color.gray;
        float dist = -1.0f;

        path = meshPath.path;
        if(path)
        {
            float totalDist = path.totalDistance;

            //Draw all points
            while (dist < totalDist)
            {
                dist = Mathf.Clamp(dist + 1.0f, 0, totalDist);
                PointPath nextPoint = path.GetPathPoint(dist);

                if (Handles.Button(transform.TransformPoint(nextPoint.point), Quaternion.identity, 0.5f, 0.5f, Handles.SphereCap))
                {
                    selectedIndexStopPoint = dist;
                    positionToDelete = -1;
                    Repaint();
                }
            }

            //Draw all stop points in blue
            Handles.color = Color.blue;
            for (int i = 0; i < path.stopPoints.Count; i++)
            {
                if (Handles.Button(transform.TransformPoint(path.GetPathPoint(path.stopPoints[i]).point), Quaternion.identity, 0.5f, 0.5f, Handles.DotCap))
                {
                    positionToDelete = path.stopPoints[i];
                    selectedIndexStopPoint = -1;
                    Repaint();
                }
            }
        }
    }
}
