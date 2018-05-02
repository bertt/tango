using UnityEngine;

namespace WhiteCat.Tween
{
	/// <summary>
	/// 左下角锚点插值动画
	/// </summary>
	[AddComponentMenu("White Cat/Tween/Rect Transform/Tween Anchor Min")]
	[RequireComponent(typeof(RectTransform))]
	public class TweenAnchorMin : TweenVector2
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
			get { return rectTransform.anchorMin; }
			set { rectTransform.anchorMin = value; }
		}

#if UNITY_EDITOR

		protected override void DrawExtraFields()
		{
			DrawClampedFromToChannels(0f, 1f);
		}

#endif // UNITY_EDITOR

	} // class TweenAnchorMin

} // namespace WhiteCat.Tween