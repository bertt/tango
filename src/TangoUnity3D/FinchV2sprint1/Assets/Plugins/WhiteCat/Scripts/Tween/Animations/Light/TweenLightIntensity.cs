using UnityEngine;

namespace WhiteCat.Tween
{
	/// <summary>
	/// 灯光强度插值动画
	/// </summary>
	[AddComponentMenu("White Cat/Tween/Light/Tween Light Intensity")]
	[RequireComponent(typeof(Light))]
	public class TweenLightIntensity : TweenFloat
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
			set { _from = Mathf.Clamp(value, 0f, 8f); }
		}


		public override float to
		{
			get { return _to; }
			set { _to = Mathf.Clamp(value, 0f, 8f); }
		}


		public override float current
		{
			get { return targetLight.intensity; }
			set { targetLight.intensity = value; }
		}

#if UNITY_EDITOR

		protected override void DrawExtraFields()
		{
			DrawClampedFromToValuesWithSlider(0f, 8f);
		}

#endif // UNITY_EDITOR

	} // class TweenLightIntensity

} // namespace WhiteCat.Tween