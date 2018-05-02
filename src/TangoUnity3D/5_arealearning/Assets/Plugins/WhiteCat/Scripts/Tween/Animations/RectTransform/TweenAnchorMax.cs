using UnityEngine;

namespace WhiteCat.Tween
{
	/// <summary>
	/// 右上角锚点插值动画
	/// </summary>
	[AddComponentMenu("White Cat/Tween/Rect Transform/Tween Anchor Max")]
	[RequireComponent(typeof(RectTransform))]
	public class TweenAnchorMax : TweenVector2
	{
		public override Vector2 from
		{
			get { return _from; }
			set
			{
				_from.x = Mathf.Clamp01(value.x);
				_from.y = Mathf.Clamp01(value.y);
			}
		}


		public override Vector2 to
		{
			get { return _to; }
			set
			{
				_to.x = Mathf.Clamp01(value.x);
				_to.y = Mathf.Clamp01(value.y);
			}
		}


		public override Vector2 current
		{
			get { return rectTransform.anchorMax; }
			set { rectTransform.anchorMax = value; }
		}

#if UNITY_EDITOR

		protected override void DrawExtraFields()
		{
			DrawClampedFromToChannels(0f, 1f);
		}

#endif // UNITY_EDITOR

	} // class TweenAnchorMax

} // namespace WhiteCat.Tween