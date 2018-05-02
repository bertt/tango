using UnityEngine;
using WhiteCat.Tween;

namespace WhiteCat.Example
{
	public class CustomTweenSize : TweenFloat
	{
		public override float current
		{
			get { return transform.localScale.x; }
			set { transform.localScale = new Vector3(value, value, value); }
		}
	}
}