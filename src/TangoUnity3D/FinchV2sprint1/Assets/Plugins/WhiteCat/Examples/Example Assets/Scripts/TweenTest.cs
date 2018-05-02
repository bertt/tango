using UnityEngine;
using WhiteCat.Tween;

namespace WhiteCat.Example
{
	public class TweenTest : Tweener.TweenAnimation
	{
		Vector3 position;


		public override void OnRecord()
		{
			position = transform.position;
		}


		public override void OnRestore()
		{
			transform.position = position;
		}


		public override void OnTween(float factor)
		{
			transform.position = new Vector3(0f, factor * 5f, 0f);
		}
	}
}