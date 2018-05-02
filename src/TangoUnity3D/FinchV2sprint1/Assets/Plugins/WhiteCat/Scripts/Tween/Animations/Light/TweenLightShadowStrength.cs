using UnityEngine;

namespace WhiteCat.Tween
{
	/// <summary>
	/// 灯光影子强度插值动画
	/// </summary>
	[AddComponentMenu("White Cat/Tween/Light/Tween Light Shadow Strength")]
	[RequireComponent(typeof(Light))]
	public class TweenLightShadowStrength : TweenFloat
	{
		Light _light;
		public Light targetLight
		{
			get
			{
				if (!_light)
				{
					_light = GetComponent<Light>();
				}
				return _light;
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
			get { return targetLight.shadowStrength; }
			set { targetLight.shadowStrength = value; }
		}

#if UNITY_EDITOR

		protected override void DrawExtraFields()
		{
			DrawClampedFromToValuesWithSlider(0f, 1f);
		}

#endif // UNITY_EDITOR

	} // class TweenLightShadowStrength

} // namespace WhiteCat.Tween