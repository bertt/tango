using UnityEngine;

namespace WhiteCat.Tween
{
	/// <summary>
	/// 雾颜色插值动画
	/// </summary>
	[AddComponentMenu("White Cat/Tween/Global/Tween Fog Color")]
	public class TweenFogColor : TweenColor
	{
		public override Color current
		{
			get { return RenderSettings.fogColor; }
			set { RenderSettings.fogColor = value; }
		}

	} // class TweenFogColor

} // namespace WhiteCat.Tween