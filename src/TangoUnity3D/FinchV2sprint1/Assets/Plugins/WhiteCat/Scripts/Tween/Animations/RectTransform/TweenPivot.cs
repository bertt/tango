using UnityEngine;

namespace WhiteCat.Tween
{
	/// <summary>
	/// pivot 插值动画
	/// </summary>
	[AddComponentMenu("White Cat/Tween/Rect Transform/Tween Pivot")]
	[RequireComponent(typeof(RectTransform))]
	public class TweenPivot : TweenVector2
	{
		public override Vector2 current
		{
			get { return rectTransform.pivot; }
			set { rectTransform.pivot = value; }
		}

	} // class TweenPivot

} // namespace WhiteCat.Tween