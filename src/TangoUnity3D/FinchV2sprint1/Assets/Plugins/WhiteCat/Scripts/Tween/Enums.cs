
namespace WhiteCat.Tween
{
	/// <summary>
	/// 循环模式
	/// </summary>
	public enum WrapMode
	{
		Clamp = 0,					// 在到达端点时保持状态不变
		Loop = 1,					// 在到达端点时跳转到另一端
		PingPong = 2,				// 在到达端点时改变方向
	}


	/// <summary>
	/// 播放模式
	/// </summary>
	public enum PlayMode
	{
		PlayForever = 0,				// 永远播放
		StopWhenForwardToEnding = 1,	// 前进到终点时停止
		StopWhenBackToBeginning = 2,	// 后退到起点时停止
		BothStop = 3,					// 前进或后退到两端时都停止
	}


	/// <summary>
	/// 材质类型
	/// </summary>
	public enum MaterialType
	{
		Specified = 0,				// 指定材质
		RendererUnique = 1,			// 渲染器独立材质
		RendererShared = 2,         // 渲染器共享材质
	}


	/// <summary>
	/// 颜色模式
	/// </summary>
	public enum ColorMode
	{
		FromTo = 0,					// 用两个颜色插值
		Gradient = 1,				// 用渐变
	}

} // namespace WhiteCat.Tween