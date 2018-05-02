using UnityEngine;

namespace WhiteCat.Tween
{
	/// <summary>
	/// offsetMin 插值动画
	/// </summary>
	[AddComponentMenu("White Cat/Tween/Rect Transform/Tween Offset Min")]
	[RequireComponent(typeof(RectTransform))]
	public class TweenOffsetMin : TweenVector2
	{
		public override Vector2 current
		{
			get { return rectTransform.offsetMin; }
			set { rectTransform.offsetMin = value; }
		}

	} // class TweenOffsetMin

} // namespace WhiteCat.Tween