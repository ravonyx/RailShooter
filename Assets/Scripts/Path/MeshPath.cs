using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(Path))]
public class MeshPath : MonoBehaviour
{
	public bool   removeDoubles = false; // removes doubles
	public Mesh   segment_sourceMesh;
	private float _segment_length;
	private float _segment_MinZ;
	private float _segment_MaxZ;

	public Path path;
	private Transform  _helpTransform1;
	private Transform  _helpTransform2;

	private MakerMesh _maker = new MakerMesh();

	// called by Button
	public void ShapeIt()
    {
		if(segment_sourceMesh == null)
        {
			Debug.LogError("missing source mesh");
			return;
		}

		_helpTransform1 = new GameObject("_helpTransform1").transform;
		_helpTransform2 = new GameObject("_helpTransform2").transform;

		// because it messes it up
		Quaternion oldRotation = transform.rotation;
		transform.rotation = Quaternion.identity;

		_maker = new MakerMesh();
		ScanSourceMesh();
		Craft(); // make segments
		Apply(); // apply values
        
		transform.rotation = oldRotation;
        
		DestroyImmediate(_helpTransform1.gameObject);
		DestroyImmediate(_helpTransform2.gameObject);
	}

	public void Craft()
    {
        path = GetComponent<Path>();
		PointPath pointA = path.GetPathPoint(0.0f);
        PointPath pointB = pointA;


        Debug.Log(path.totalDistance);
		for(float dist=0.0f; dist< path.totalDistance; dist+=_segment_length)
        {
			pointB = path.GetPathPoint(Mathf.Clamp(dist + _segment_length,0, path.totalDistance));

			_helpTransform1.rotation = Quaternion.LookRotation(pointA.forward, pointA.up);
			_helpTransform1.position = transform.TransformPoint(pointA.point);

			_helpTransform2.rotation = Quaternion.LookRotation(pointB.forward, pointB.up);
			_helpTransform2.position = transform.TransformPoint(pointB.point);

			Add_Segment();
			pointA = pointB;
		}
	}

	public void Add_Segment()
    {
		int[] indices;
		// go throught the submeshes
		for(int sub=0; sub<segment_sourceMesh.subMeshCount; sub++)
        {
			indices = segment_sourceMesh.GetIndices(sub);
			for(int i=0; i<indices.Length; i+=3)
            {
				AddTriangle(new int[]{indices[i], indices[i+1], indices[i+2]},sub);
			}
		}
	}

	public void AddTriangle( int[] indices, int submesh)
    {
		// vertices
		Vector3[] verts = new Vector3[3]{
			segment_sourceMesh.vertices[indices[0]],
			segment_sourceMesh.vertices[indices[1]],
			segment_sourceMesh.vertices[indices[2]]
		};
		// normals
		Vector3[] norms = new Vector3[3]{
			segment_sourceMesh.normals[indices[0]],
			segment_sourceMesh.normals[indices[1]],
			segment_sourceMesh.normals[indices[2]]
		};
		// uvs
		Vector2[] uvs = new Vector2[3]{
			segment_sourceMesh.uv[indices[0]],
			segment_sourceMesh.uv[indices[1]],
			segment_sourceMesh.uv[indices[2]]
		};
		// tangent
		Vector4[] tangents = new Vector4[3]{
			segment_sourceMesh.tangents[indices[0]],
			segment_sourceMesh.tangents[indices[1]],
			segment_sourceMesh.tangents[indices[2]]
		};

		// apply offset
		float lerpValue = 0.0f;
		Vector3 pointA, pointB;
		Vector3 normA, normB;
		Vector4 tangentA, tangentB;
		Matrix4x4 localToWorld_A = _helpTransform1.localToWorldMatrix;
		Matrix4x4 localToWorld_B = _helpTransform2.localToWorldMatrix;
		Matrix4x4 worldToLocal =   transform.worldToLocalMatrix;
		for(int i=0; i<3; i++){

			lerpValue = RemapValue(verts[i].z, _segment_MinZ, _segment_MaxZ, 0.0f, 1.0f);
			verts[i].z = 0.0f;
					
			pointA = localToWorld_A.MultiplyPoint(verts[i]); // to world
			pointB = localToWorld_B.MultiplyPoint(verts[i]);

			verts[i] = worldToLocal.MultiplyPoint(Vector3.Lerp(pointA, pointB,lerpValue)); // to local

			normA = localToWorld_A.MultiplyVector(norms[i]);
			normB = localToWorld_B.MultiplyVector(norms[i]);

			norms[i] = worldToLocal.MultiplyVector(Vector3.Lerp(normA, normB, lerpValue));

			tangentA = localToWorld_A.MultiplyVector(tangents[i]);
			tangentB = localToWorld_B.MultiplyVector(tangents[i]);

			tangents[i] = worldToLocal.MultiplyVector(Vector3.Lerp(tangentA, tangentB, lerpValue));

		}
		_maker.AddTriangle(verts, norms, uvs, tangents, submesh);
	}

	public void ScanSourceMesh()
    {
		float min_z = 0.0f, max_z = 0.0f;
		// find length
		for(int i=0; i<segment_sourceMesh.vertexCount;i++)
        {
			Vector3 vert = segment_sourceMesh.vertices[i];
			if(vert.z < min_z)
				min_z = vert.z;

			if(vert.z > max_z)
				max_z = vert.z;
		}
		_segment_MinZ = min_z;
		_segment_MaxZ = max_z;
		_segment_length = max_z - min_z;
	}

	public void Apply()
    {
		if(removeDoubles)
			_maker.RemoveDoubles();
		GetComponent<MeshFilter>().mesh = _maker.GetMesh();
	}

    public static float RemapValue(float value, float oldMin, float oldMax, float newMin, float newMax)
    {
        float returnValue = 0;
        returnValue = ((value - oldMin) / (oldMax - oldMin)) * (newMax - newMin) + newMin;
        return returnValue;
    }
}
