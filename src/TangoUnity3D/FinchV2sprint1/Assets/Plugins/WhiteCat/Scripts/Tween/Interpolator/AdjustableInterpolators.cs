
namespace WhiteCat.Tween
{
	/// <summary>
	/// 可调节参数的插值方法
	/// </summary>
	public partial class Interpolator
	{
		/// <summary>
		/// 对称插值
		/// </summary>
		/// <param name="f">
		/// 平缓系数.
		/// (-10, -2): Back In Out
		/// (-2, 23): No Ease or Back
		/// </param>
		/// <param name="t"> 单位化的时间, 即 [0, 1] 范围的数值 </param>
		/// <returns> 插值结果 </returns>
		public static float Symmetric(float f, float t)
		{
			return (((t - 1.5f) * t + 0.5f) * f + 1f) * t;
		}


		/// <summary>
		/// 非对称插值
		/// </summary>
		/// <param name="f">
		/// 平缓系数.
		/// (-3, -1): Back Out
		/// (-1, 1): No Ease or Back
		/// (1, 3): Back In
		/// </param>
		/// <param name="t"> 单位化的时间, 即 [0, 1] 范围的数值 </param>
		/// <returns> 插值结果 </returns>
		public static float Asymmetric(float f, float t)
		{
			return (f * t + 1f - f) * t;
		}


		/// <summary>
		/// 平缓起步插值
		/// </summary>
		/// <param name="f">
		/// 平缓系数.
		/// (-5.2, -2): Ease In; Back Out
		/// (-2, 1): Ease In
		/// (1, 4.4): Ease In; Back In
		/// </param>
		/// <param name="t"> 单位化的时间, 即 [0, 1] 范围的数值 </param>
		/// <returns> 插值结果 </returns>
		public static float EaseIn(float f, float t)
		{
			return t * t * (1f - f * (1f - t));
		}


		/// <summary>
		/// 平缓终止插值
		/// </summary>
		/// <param name="f">
		/// 平缓系数.
		/// (-5.2, -2): Ease Out; Back In
		/// (-2, 1): Ease Out
		/// (1, 4.4): Ease Out; Back Out
		/// </param>
		/// <param name="t"> 单位化的时间, 即 [0, 1] 范围的数值 </param>
		/// <returns> 插值结果 </returns>
		public static float EaseOut(float f, float t)
		{
			t = 1f - t;
			return 1f - t * t * (1f - f * (1f - t));
		}


		/// <summary>
		/// 平缓起止的对称插值
		/// </summary>
		/// <param name="f">
		/// 平缓系数.
		/// (-123, 6): Ease In Out
		/// (6, 52): Ease In Out; Back In Out
		/// </param>
		/// <param name="t"> 单位化的时间, 即 [0, 1] 范围的数值 </param>
		/// <returns> 插值结果 </returns>
		public static float EaseInOutSymmetric(float f, float t)
		{
			return t * t * (f * (t * t * (t - 2.5f) + t + t - 0.5f) + 3f - t - t);
		}


		/// <summary>
		/// 平缓起止的不对称插值
		/// </summary>
		/// <param name="f">
		/// 平缓系数.
		/// (-12, -3): Ease In Out; Back In
		/// (-3, 3): Ease In Out
		/// (3, 12): Ease In Out; Back Out
		/// </param>
		/// <param name="t"> 单位化的时间, 即 [0, 1] 范围的数值 </param>
		/// <returns> 插值结果 </returns>
		public static float EaseInOutAsymmetric(float f, float t)
		{
			return t * t * (t * (f * t - 2f - f - f) + f + 3f);
		}


		/// <summary>
		/// 平缓起步且起止反弹的插值
		/// </summary>
		/// <param name="f">
		/// 平缓系数.
		/// (-21, -3): Ease In; Back In Out
		/// </param>
		/// <param name="t"> 单位化的时间, 即 [0, 1] 范围的数值 </param>
		/// <returns> 插值结果 </returns>
		public static float EaseInBackInOut(float f, float t)
		{
			float b = -1.65f * f - 0.95f;
			return ((f * t + b) * t + 1 - f - b) * t * t;
		}


		/// <summary>
		/// 平缓终止且起止反弹的插值
		/// </summary>
		/// <param name="f">
		/// 平缓系数.
		/// (-21, -3): Ease Out; Back In Out
		/// </param>
		/// <param name="t"> 单位化的时间, 即 [0, 1] 范围的数值 </param>
		/// <returns> 插值结果 </returns>
		public static float EaseOutBackInOut(float f, float t)
		{
			t = 1f - t;
			float b = -1.65f * f - 0.95f;
			return 1f - ((f * t + b) * t + 1 - f - b) * t * t;
		}

	} // class Interpolator

} // namespace WhiteCat.Tween