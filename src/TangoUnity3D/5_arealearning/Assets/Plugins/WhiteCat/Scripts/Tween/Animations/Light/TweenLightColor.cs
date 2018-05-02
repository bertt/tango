using UnityEngine;

namespace WhiteCat.Tween
{
	/// <summary>
	/// 灯光颜色插值动画
	/// </summary>
	[AddComponentMenu("White Cat/Tween/Light/Tween Light Color")]
	[RequireComponent(typeof(Light))]
	public class TweenLightColor : TweenColor
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


		public override Color current
		{
			get { return targetLight.color; }
			set { targetLight.color = value; }
		}

	} // class TweenLightColor

} // namespace WhiteCat.Tween