using UnityEngine;
using System.Collections.Generic;
using WhiteCat;
using WhiteCat.Paths;

namespace WhiteCat.Example
{
	[RequireComponent(typeof(MoveAlongPath))]
	public class BuildMeshAlongPath : MonoBehaviour
	{
		[Range(1f, 50f)]
		public float width = 6;
		[Range(0.5f, 5f)]
		public float deltaAngle = 3;
		[Range(0.01f, 1f)]
		public float uvxPerUnit = 0.1f;
		[Range(0.01f, 1f)]
		public float stepDistance = 0.05f;


		public void Build()
		{
			List<Vector3> vertices = new List<Vector3>();
			List<Vector3> normals = new List<Vector3>();
			List<Vector2> uv = new List<Vector2>();
			List<int> triangles = new List<int>();

			var builder = GetComponent<MoveAlongPath>();
			var transform = builder.transform;
			float length = builder.path.length;
			builder.distance = 0f;

			vertices.Add(transform.position - transform.right * (width * 0.5f));
			vertices.Add(transform.position + transform.right * (width * 0.5f));
			normals.Add(transform.up);
			normals.Add(transform.up);
			uv.Add(new Vector2(0, 1));
			uv.Add(new Vector2(0, 0));

			var originalRotation = transform.rotation;

			while (builder.distance < length)
			{
				builder.distance = Mathf.Clamp(builder.distance + stepDistance, 0f, length);

				if (Quaternion.Angle(originalRotation, transform.rotation) >= deltaAngle || builder.distance == length)
				{
					originalRotation = transform.rotation;

					vertices.Add(transform.position - transform.right * (width * 0.5f));
					vertices.Add(transform.position + transform.right * (width * 0.5f));
					normals.Add(transform.up);
					normals.Add(transform.up);
					uv.Add(new Vector2(builder.distance * uvxPerUnit, 1));
					uv.Add(new Vector2(builder.distance * uvxPerUnit, 0));
					triangles.Add(vertices.Count - 4);
					triangles.Add(vertices.Count - 2);
					triangles.Add(vertices.Count - 3);
					triangles.Add(vertices.Count - 3);
					triangles.Add(vertices.Count - 2);
					triangles.Add(vertices.Count - 1);
				}
			}

			Mesh mesh = new Mesh();
			mesh.vertices = vertices.ToArray();
			mesh.normals = normals.ToArray();
			mesh.uv = uv.ToArray();
			mesh.triangles = triangles.ToArray();
			mesh.RecalculateBounds();

			Kit.SafeGetComponent<MeshFilter>(builder.path.gameObject).sharedMesh = mesh;
		}
	}
}