using UnityEngine;

namespace WhiteCat.Tween
{
	/// <summary>
	/// 雾 End Distance 插值动画 (仅对 Linear 模式生效)
	/// </summary>
	[AddComponentMenu("White Cat/Tween/Global/Tween Fog End Distance")]
	public class TweenFogEndDistance : TweenFloat
	{
		public override float current
		{
			get { return RenderSettings.fogEndDistance; }
			set { RenderSettings.fogEndDistance = value; }
		}

	} // class TweenFogEndDistance

} // namespace WhiteCat.Tween