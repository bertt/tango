using UnityEngine;

namespace WhiteCat.Tween
{
	/// <summary>
	/// 渐变类型全局光底部颜色插值动画
	/// </summary>
	[AddComponentMenu("White Cat/Tween/Global/Tween Ambient Ground Color")]
	public class TweenAmbientGroundColor : TweenColor
	{
		public override Color current
		{
			get { return RenderSettings.ambientGroundColor; }
			set { RenderSettings.ambientGroundColor = value; }
		}

	} // class TweenAmbientGroundColor

} // namespace WhiteCat.Tween