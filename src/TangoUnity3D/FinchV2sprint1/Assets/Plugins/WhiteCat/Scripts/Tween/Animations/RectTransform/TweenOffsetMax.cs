using UnityEngine;

namespace WhiteCat.Tween
{
	/// <summary>
	/// offsetMax 插值动画
	/// </summary>
	[AddComponentMenu("White Cat/Tween/Rect Transform/Tween Offset Max")]
	[RequireComponent(typeof(RectTransform))]
	public class TweenOffsetMax : TweenVector2
	{
		public override Vector2 current
		{
			get { return rectTransform.offsetMax; }
			set { rectTransform.offsetMax = value; }
		}

	} // class TweenOffsetMax

} // namespace WhiteCat.Tween