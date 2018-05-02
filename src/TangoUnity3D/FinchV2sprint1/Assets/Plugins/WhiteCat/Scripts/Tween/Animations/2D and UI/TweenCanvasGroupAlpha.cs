using UnityEngine;

namespace WhiteCat.Tween
{
	/// <summary>
	/// Canvas Group 不透明度插值动画
	/// </summary>
	[AddComponentMenu("White Cat/Tween/2D and UI/Tween Canvas Group Alpha")]
	[RequireComponent(typeof(CanvasGroup))]
	public class TweenCanvasGroupAlpha : TweenFloat
	{
		CanvasGroup _canvasGroup;
		public CanvasGroup canvasGroup
		{
			get
			{
				if (!_canvasGroup)
				{
					_canvasGroup = GetComponent<CanvasGroup>();
				}
				return _canvasGroup;
			}
		}


		public override float from
		{
			get { return _from; }
			set { _from = Mathf.Clamp01(value); }
		}


		public override float to
		{
			get { return _to; }
			set { _to = Mathf.Clamp01(value); }
		}


		public override float current
		{
			get { return canvasGroup.alpha; }
			set { canvasGroup.alpha = value; }
		}

#if UNITY_EDITOR

		protected override void DrawExtraFields()
		{
			DrawClampedFromToValuesWithSlider(0f, 1f);
		}

#endif // UNITY_EDITOR

	} // class TweenCanvasGroupAlpha

} // namespace WhiteCat.Tween