using UnityEngine;

namespace WhiteCat.Tween
{
	/// <summary>
	/// 缩放插值动画
	/// </summary>
	[AddComponentMenu("White Cat/Tween/Transform/Tween Scale")]
	public class TweenScale : TweenVector3
	{
		public override Vector3 current
		{
			get { return transform.localScale; }
			set { transform.localScale = value; }
		}

	} // class TweenScale

} // namespace WhiteCat.Tween