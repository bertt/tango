using UnityEngine;

namespace WhiteCat.Tween
{
	/// <summary>
	/// 雾 Start Distance 插值动画 (仅对 Linear 模式生效)
	/// </summary>
	[AddComponentMenu("White Cat/Tween/Global/Tween Fog Start Distance")]
	public class TweenFogStartDistance : TweenFloat
	{
		public override float current
		{
			get { return RenderSettings.fogStartDistance; }
			set { RenderSettings.fogStartDistance = value; }
		}

	} // class TweenFogStartDistance

} // namespace WhiteCat.Tween