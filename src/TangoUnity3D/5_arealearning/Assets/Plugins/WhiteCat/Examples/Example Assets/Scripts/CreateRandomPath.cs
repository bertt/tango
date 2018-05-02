using UnityEngine;
using WhiteCat;
using WhiteCat.Paths;

namespace WhiteCat.Example
{
	[RequireComponent(typeof(CardinalPath))]
	public class CreateRandomPath : MonoBehaviour
	{
		[Range(4, 32)]
		public int nodeCount = 16;
		[Range(20, 60)]
		public float minRadius = 25;
		[Range(20, 60)]
		public float maxRadius = 55;
		[Range(0, 20)]
		public float maxHeight = 10;


		public void Generate()
		{
			Vector3[] nodes = new Vector3[nodeCount];

			Vector3 point;
			float radian;
			float radius;

			for (int i = 0; i < nodeCount; i++)
			{
				radian = 2 * Mathf.PI * i / nodeCount;
				radius = UnityEngine.Random.Range(minRadius, maxRadius);

				point.x = Mathf.Cos(radian) * radius;
				point.y = UnityEngine.Random.Range(0, maxHeight);
				point.z = Mathf.Sin(radian) * radius;

				nodes[i] = point;
			}

			var path = GetComponent<CardinalPath>();
			path.SetNodes(nodes, true);
		}
	}
}