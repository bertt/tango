using UnityEngine;

namespace WhiteCat.Tween
{
	/// <summary>
	/// 雾密度插值动画 (仅对 Exponential 和 ExponentialSquared 模式生效)
	/// </summary>
	[AddComponentMenu("White Cat/Tween/Global/Tween Fog Density")]
	public class TweenFogDensity : TweenFloat
	{
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
			get { return RenderSettings.fogDensity; }
			set { RenderSettings.fogDensity = value; }
		}

#if UNITY_EDITOR

		protected override void DrawExtraFields()
		{
			DrawClampedFromToValuesWithSlider(0f, 1f);
		}

#endif // UNITY_EDITOR

	} // class TweenFogDensity

} // namespace WhiteCat.Tween