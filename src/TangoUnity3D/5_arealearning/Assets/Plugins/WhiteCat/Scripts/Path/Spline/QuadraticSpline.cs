using System;
using UnityEngine;

namespace WhiteCat.Paths
{
	/// <summary>
	/// 二次样条
	/// </summary>
	[Serializable]
	public class QuadraticSpline
	{
		public Vector3 f0;           // t^0 系数
		public Vector3 f1;           // t^1 系数
		public Vector3 f2;           // t^2 系数


		/// <summary>
		/// Bezier Curve
		/// </summary>
		public void SetBezierCurve(Vector3 p0, Vector3 p1, Vector3 p2)
		{
			f0 = p0;
			f1 = 2f * (p1 - p0);
			f2 = p2 - p1 - p1 + p0;
		}


		/// <summary>
		/// 获取点坐标
		/// </summary>
		public Vector3 GetPoint(float t)
		{
			return f0 + t * f1 + t * t * f2;
		}


		/// <summary>
		/// 一阶导函数
		/// </summary>
		public Vector3 GetDerivative(float t)
		{
			return f1 + 2f * t * f2;
		}


		/// <summary>
		/// 二阶导函数
		/// </summary>
		public Vector3 GetSecondDerivative(float t)
		{
			return f2 + f2;
		}


		/// <summary>
		/// 获取长度
		/// </summary>
		public float GetLength(float t)
		{
			float A = f2.sqrMagnitude * 4f;
			float B = Vector3.Dot(f1, f2) * 4f;
			float C = f1.sqrMagnitude;

			float M = 2f * Mathf.Sqrt(A * (A * t * t + B * t + C));
			float N = 2f * Mathf.Sqrt(A * C);

			if (A < Kit.OneMillionth)
			{
				return C * 0.5f;
			}

			if (B + N < Kit.OneMillionth)
			{
				return (2f * M * t * A + (M - N) * B) / (8f * A * Mathf.Sqrt(A));
			}

			return
				( 2f * M * t * A + (M - N) * B
					+ (B * B - 4f * A * C) * Mathf.Log((B + N) / (B + 2f * A * t + M))
				) / (8f * A * Mathf.Sqrt(A));
		}

	} // class QuadraticSpline

} // namespace WhiteCat.Paths