using UnityEngine;

namespace WhiteCat.Tween
{
	/// <summary>
	/// 灯光范围插值动画
	/// </summary>
	[AddComponentMenu("White Cat/Tween/Light/Tween Light Range")]
	[RequireComponent(typeof(Light))]
	public class TweenLightRange : TweenFloat
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
			set { _from = value > 0f ? value : 0f; }
		}


		public override float to
		{
			get { return _to; }
			set { _to = value > 0f ? value : 0f; }
		}


		public override float current
		{
			get { return targetLight.range; }
			set { targetLight.range = value; }
		}

#if UNITY_EDITOR

		protected override void DrawExtraFields()
		{
			DrawClampedFromToValues(0f, Kit.Million);
		}

#endif // UNITY_EDITOR

	} // class TweenLightRange

} // namespace WhiteCat.Tween