using System;
using UnityEngine;

namespace WhiteCat
{
	/// <summary>
	/// 数学相关的方法
	/// </summary>
	public partial struct Kit
	{
		/// <summary>
		/// 2 的平方根
		/// </summary>
		public const float Sqrt2 = 1.41421356f;


		/// <summary>
		/// 3 的平方根
		/// </summary>
		public const float Sqrt3 = 1.73205081f;


		/// <summary>
		/// 百万分之一
		/// </summary>
		public const float OneMillionth = 1e-6f;


		/// <summary>
		/// 一百万
		/// </summary>
		public const float Million = 1e6f;


		/// <summary>
		/// 保留指定的有效位数, 对剩余部分四舍五入
		/// </summary>
		public static double RoundToSignificantDigits(double value, int digits = 15)
		{
			if (value == 0.0) return 0.0;

			double scale = Math.Pow(10.0, Math.Floor(Math.Log10(Math.Abs(value))) + 1);
			return scale * Math.Round(value / scale, digits);
		}


		/// <summary>
		/// 保留指定的有效位数, 对剩余部分四舍五入
		/// </summary>
		public static float RoundToSignificantDigitsFloat(float value, int digits = 6)
		{
			return (float)RoundToSignificantDigits((double)value, digits);
		}


		/// <summary>
		/// 将参数单位化
		/// </summary>
		/// <returns> 正值返回 1, 负值返回 -1, 0 返回 0 </returns>
		public static float Normalize(float value)
		{
			if (value > 0f) return 1f;
			if (value < 0f) return -1f;
			return 0f;
		}


		/// <summary>
		/// 反转插值
		/// </summary>
		/// <param name="t"> 单位化的时间, 即取值范围为 [0, 1] </param>
		/// <param name="interpolate"> 一个在 [0, 1] 上的插值方法 </param>
		/// <returns> 与给定插值方法关于 (0.5, 0.5) 中心对称的插值方法的插值结果 </returns>
		public static float InverseInterpolate(float t, Func<float, float> interpolate)
		{
			return 1f - interpolate(1f - t);
		}


		/// <summary>
		/// 分三阶段的插值
		/// </summary>
		/// <param name="t"> 单位化的时间, 即取值范围为 [0, 1] </param>
		/// <param name="t1"> 第一个时间点 </param>
		/// <param name="t2"> 第二个时间点 </param>
		/// <param name="interpolate"> 一个在 [0, 1] 上的插值方法 </param>
		/// <returns> [0, t1) 返回插值方法前一半，(t2, 1] 返回插值方法后一半，[t1, t2] 返回 0.5 处的值 </returns>
		public static float InterpolateInThreePhases(
			float t,
			float t1,
			float t2,
			Func<float, float> interpolate)
		{
			if (t2 < t1) Kit.Swap(ref t1, ref t2);

			if (t < t1) return interpolate(t / t1 * 0.5f);

			if (t > t2) return interpolate((t - t2) / (1.0f - t2) * 0.5f + 0.5f);

			return interpolate(0.5f);
		}


		/// <summary>
		/// 基数样条
		/// </summary>
		public static float CardinalSpline(
			float p0, float p1, float p2, float p3,
			float t,
			float tension = 0.5f)
		{
			float a = tension * (p2 - p0);
			float b = p2 - p1;
			float c = tension * (p3 - p1) + a - b - b;

			return p1 + t * a - t * t * (a + c - b) + t * t * t * c;
		}


		/// <summary>
		/// 基数样条
		/// </summary>
		public static Vector3 CardinalSpline(
			Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3,
			float t,
			float tension = 0.5f)
		{
			Vector3 a = tension * (p2 - p0);
			Vector3 b = p2 - p1;
			Vector3 c = tension * (p3 - p1) + a - b - b;

			return p1 + t * a - t * t * (a + c - b) + t * t * t * c;
		}


		/// <summary>
		/// 获得线段上距离指定点最近的点
		/// </summary>
		public static float ClosestPoint01(Vector3 start, Vector3 end, Vector3 point)
		{
			Vector3 direction = end - start;

			float t = direction.sqrMagnitude;
			if (t < OneMillionth) return 0f;

			return Mathf.Clamp01(Vector3.Dot(point - start, direction) / t);
		}


		/// <summary>
		/// 获得线段上距离指定点最近的点
		/// </summary>
		public static Vector3 ClosestPoint(Vector3 start, Vector3 end, Vector3 point)
		{
			return start + (end - start) * ClosestPoint01(start, end, point);
		}


		/// <summary>
		/// 获得两条线段上最近的两点
		/// </summary>
		public static void ClosestPoint(
			Vector3 startA, Vector3 endA,
			Vector3 startB, Vector3 endB,
			out Vector3 pointA, out Vector3 pointB)
		{
			Vector3 directionA = endA - startA;
			Vector3 directionB = endB - startB;

			float k0 = Vector3.Dot(directionA, directionB);
			float k1 = directionA.sqrMagnitude;
			float k2 = Vector3.Dot(startA - startB, directionA);
			float k3 = directionB.sqrMagnitude;
			float k4 = Vector3.Dot(startA - startB, directionB);

			float t = k3 * k1 - k0 * k0;
			float a = (k0 * k4 - k3 * k2) / t;
			float b = (k1 * k4 - k0 * k2) / t;

			if (float.IsNaN(a) || float.IsNaN(b))
			{
				pointB = ClosestPoint(startB, endB, startA);
				pointA = ClosestPoint(startB, endB, endA);

				if ((pointB - startA).sqrMagnitude < (pointA - endA).sqrMagnitude)
				{
					pointA = startA;
				}
				else
				{
					pointB = pointA;
					pointA = endA;
				}
				return;
			}

			if (a < 0f)
			{
				if (b < 0f)
				{
					pointA = ClosestPoint(startA, endA, startB);
					pointB = ClosestPoint(startB, endB, startA);

					if ((pointA - startB).sqrMagnitude < (pointB - startA).sqrMagnitude)
					{
						pointB = startB;
					}
					else pointA = startA;
				}
				else if (b > 1f)
				{
					pointA = ClosestPoint(startA, endA, endB);
					pointB = ClosestPoint(startB, endB, startA);

					if ((pointA - endB).sqrMagnitude < (pointB - startA).sqrMagnitude)
					{
						pointB = endB;
					}
					else pointA = startA;
				}
				else
				{
					pointA = startA;
					pointB = ClosestPoint(startB, endB, startA);
				}
			}
			else if (a > 1f)
			{
				if (b < 0f)
				{
					pointA = ClosestPoint(startA, endA, startB);
					pointB = ClosestPoint(startB, endB, endA);

					if ((pointA - startB).sqrMagnitude < (pointB - endA).sqrMagnitude)
					{
						pointB = startB;
					}
					else pointA = endA;
				}
				else if (b > 1f)
				{
					pointA = ClosestPoint(startA, endA, endB);
					pointB = ClosestPoint(startB, endB, endA);

					if ((pointA - endB).sqrMagnitude < (pointB - endA).sqrMagnitude)
					{
						pointB = endB;
					}
					else pointA = endA;
				}
				else
				{
					pointA = endA;
					pointB = ClosestPoint(startB, endB, endA);
				}
			}
			else
			{
				if (b < 0f)
				{
					pointB = startB;
					pointA = ClosestPoint(startA, endA, startB);
				}
				else if (b > 1f)
				{
					pointB = endB;
					pointA = ClosestPoint(startA, endA, endB);
				}
				else
				{
					pointA = startA + a * directionA;
					pointB = startB + b * directionB;
				}
			}
		}


		/// <summary>
		/// 射线检测平面
		/// </summary>
		/// <param name="rayOrigin"> 射线起点 </param>
		/// <param name="rayDirection"> 射线方向 </param>
		/// <param name="planePoint"> 面内一点 </param>
		/// <param name="planeNormal"> 面法线 </param>
		/// <returns> 返回 -1 表示不相交, 否则将返回值带入 rayOrigin + rayDirection * t 即可求出交点 </returns>
		public static float RayIntersectPlane(Vector3 rayOrigin, Vector3 rayDirection, Vector3 planePoint, Vector3 planeNormal)
		{
			float t = Vector3.Dot(planeNormal, rayDirection);

			if (t > -OneMillionth) return -1f;

			return Vector3.Dot(planePoint - rayOrigin, planeNormal) / t;
		}


		/// <summary>
		/// 从矩阵中获取位置
		/// </summary>
		public static Vector3 GetPositionOfMatrix(ref Matrix4x4 matrix)
		{
			return new Vector3(matrix.m03, matrix.m13, matrix.m23);
		}


		/// <summary>
		/// 从矩阵中获取旋转
		/// </summary>
		public static Quaternion GetRotationOfMatrix(ref Matrix4x4 matrix)
		{
			return Quaternion.LookRotation(
				new Vector3(matrix.m02, matrix.m12, matrix.m22),
				new Vector3(matrix.m01, matrix.m11, matrix.m21)
				);
		}


		/// <summary>
		/// 从矩阵中获取缩放
		/// </summary>
		public static Vector3 GetScaleOfMatrix(ref Matrix4x4 matrix)
		{
			return new Vector3(
				new Vector4(matrix.m00, matrix.m10, matrix.m20, matrix.m30).magnitude,
				new Vector4(matrix.m01, matrix.m11, matrix.m21, matrix.m31).magnitude,
				new Vector4(matrix.m02, matrix.m12, matrix.m22, matrix.m32).magnitude
				);
		}

	} // struct Kit

} // namespace WhiteCat