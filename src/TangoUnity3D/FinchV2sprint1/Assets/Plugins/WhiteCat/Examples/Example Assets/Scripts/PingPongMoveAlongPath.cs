using UnityEngine;
using WhiteCat.Paths;

namespace WhiteCat.Example
{
	[RequireComponent(typeof(MoveAlongPathWithSpeed))]
	public class PingPongMoveAlongPath : MonoBehaviour
	{
		MoveAlongPathWithSpeed move;


		void Awake()
		{
			move = GetComponent<MoveAlongPathWithSpeed>();
		}


		void LateUpdate()
		{
			if (move.distance <= 0f)
			{
				move.speed = 1.25f;
			}
			else if (move.distance >= move.path.length)
			{
				move.speed = -1.25f;
			}
		}
	}
}