using UnityEngine;

namespace WhiteCat.Tween
{
	/// <summary>
	/// 环境光强度动画
	/// </summary>
	[AddComponentMenu("White Cat/Tween/Global/Tween Ambient Intensity")]
	public class TweenAmbientIntensity : TweenFloat
	{
		public override float from
		{
			get { return _from; }
			set { _from = Mathf.Clamp(value, 0f, 8f); }
		}


		public override float to
		{
			get { return _to; }
			set { _to = Mathf.Clamp(value, 0f, 8f); }
		}


		public override float current
		{
			get { return RenderSettings.ambientIntensity; }
			set { RenderSettings.ambientIntensity = value; }
		}

#if UNITY_EDITOR

		protected override void DrawExtraFields()
		{
			DrawClampedFromToValuesWithSlider(0f, 8f);
		}

#endif // UNITY_EDITOR

	} // class TweenAmbientIntensity

} // namespace WhiteCat.Tween