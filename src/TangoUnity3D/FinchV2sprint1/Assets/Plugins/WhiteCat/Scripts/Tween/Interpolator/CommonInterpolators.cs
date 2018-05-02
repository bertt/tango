using UnityEngine;

namespace WhiteCat.Tween
{
	/// <summary>
	/// 常用插值方法
	/// </summary>
	public partial class Interpolator
	{
		/// <summary>
		/// 线性插值
		/// </summary>
		/// <param name="t"> 单位化的时间, 即 [0, 1] 范围的数值 </param>
		/// <returns> 插值结果 </returns>
		public static float Linear(float t)
		{
			return t;
		}


		/// <summary>
		/// 加速插值
		/// </summary>
		/// <param name="t"> 单位化的时间, 即 [0, 1] 范围的数值 </param>
		/// <returns> 插值结果 </returns>
		public static float Accelerate(float t)
		{
			return t * t;
		}


		/// <summary>
		/// 减速插值
		/// </summary>
		/// <param name="t"> 单位化的时间, 即 [0, 1] 范围的数值 </param>
		/// <returns> 插值结果 </returns>
		public static float Decelerate(float t)
		{
			return (2f - t) * t;
		}


		/// <summary>
		/// 先加速后减速插值
		/// </summary>
		/// <param name="t"> 单位化的时间, 即 [0, 1] 范围的数值 </param>
		/// <returns> 插值结果 </returns>
		public static float AccelerateDecelerate(float t)
		{
			return (3f - t - t) * t * t;
		}


		/// <summary>
		/// 反弹加速插值
		/// </summary>
		/// <param name="t"> 单位化的时间, 即 [0, 1] 范围的数值 </param>
		/// <returns> 插值结果 </returns>
		public static float Anticipate(float t)
		{
			return (t + t + t - 2f) * t * t;
		}


		/// <summary>
		/// 减速反弹插值
		/// </summary>
		/// <param name="t"> 单位化的时间, 即 [0, 1] 范围的数值 </param>
		/// <returns> 插值结果 </returns>
		public static float Overshoot(float t)
		{
			t = 1f - t;
			return 1f - (t + t + t - 2f) * t * t;
		}


		/// <summary>
		/// 先反弹加速后减速反弹插值
		/// </summary>
		/// <param name="t"> 单位化的时间, 即 [0, 1] 范围的数值 </param>
		/// <returns> 插值结果 </returns>
		public static float AnticipateOvershoot(float t)
		{
			return ((((28f * t - 70f) * t + 54f) * t - 11f) * t) * t;
		}


		/// <summary>
		/// 弹跳插值
		/// </summary>
		/// <param name="t"> 单位化的时间, 即 [0, 1] 范围的数值 </param>
		/// <returns> 插值结果 </returns>
		public static float Bounce(float t)
		{
			if (t < 1f / 3f) return 9f * t * t;
			if (t < 2f / 3f) return 9f * t * (t - 1f) + 3f;
			if (t < 13f / 15f) return t * (9f * t - 13.8f) + 6.2f;
			return t * (9f * t - 16.8f) + 8.8f;
		}


		/// <summary>
		/// 抛物线插值
		/// </summary>
		/// <param name="t"> 单位化的时间, 即 [0, 1] 范围的数值 </param>
		/// <returns> 插值结果 </returns>
		public static float Parabolic(float t)
		{
			return 4f * t * (1f - t);
		}


		/// <summary>
		/// 正弦插值
		/// </summary>
		/// <param name="t"> 单位化的时间, 即 [0, 1] 范围的数值 </param>
		/// <returns> 插值结果 </returns>
		public static float Sine(float t)
		{
			return Mathf.Sin((t + t + 1.5f) * Mathf.PI) * 0.5f + 0.5f;
		}

	} // class Interpolator

} // namespace WhiteCat.Tween