using UnityEngine;

namespace WhiteCat.Tween
{
	/// <summary>
	/// 相对于锚点的 3D 位置插值动画
	/// </summary>
	[AddComponentMenu("White Cat/Tween/Rect Transform/Tween Anchored Position 3D")]
	[RequireComponent(typeof(RectTransform))]
	public class TweenAnchoredPosition3D : TweenVector3
	{
		public override Vector3 current
		{
			get { return rectTransform.anchoredPosition3D; }
			set { rectTransform.anchoredPosition3D = value; }
		}

	} // class TweenAnchoredPosition3D

} // namespace WhiteCat.Tween