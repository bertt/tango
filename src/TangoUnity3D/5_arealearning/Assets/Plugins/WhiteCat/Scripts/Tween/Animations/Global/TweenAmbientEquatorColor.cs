using UnityEngine;

namespace WhiteCat.Tween
{
	/// <summary>
	/// 渐变类型全局光中部颜色插值动画
	/// </summary>
	[AddComponentMenu("White Cat/Tween/Global/Tween Ambient Equator Color")]
	public class TweenAmbientEquatorColor : TweenColor
	{
		public override Color current
		{
			get { return RenderSettings.ambientEquatorColor; }
			set { RenderSettings.ambientEquatorColor = value; }
		}

	} // class TweenAmbientEquatorColor

} // namespace WhiteCat.Tween