using UnityEngine;
using WhiteCat.Paths;

namespace WhiteCat.Example
{
	[RequireComponent(typeof(MoveAlongPathPhysics))]
	public class DragAlongPath : MonoBehaviour
	{
		public float forceScale = 0.1f;
		public Vector3 gravity = new Vector3(0f, -10f, 0f);

		MoveAlongPathPhysics _move;


		void Awake()
		{
			_move = GetComponent<MoveAlongPathPhysics>();
		}


		void Update()
		{
			if (Input.GetMouseButton(0))
			{
				var point = Camera.main.WorldToScreenPoint(transform.position);
				_move.force = (Input.mousePosition - point) * forceScale + gravity;

				Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				mouse.z = 0f;
				Debug.DrawLine(transform.position, mouse, Color.white);
			}
			else
			{
				_move.force = gravity;
			}
		}
	}
}