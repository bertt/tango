using UnityEngine;

namespace WhiteCat
{
	/// <summary>
	/// 轴
	/// </summary>
	public enum Axis
	{
		None = 0,

		PositiveX = 1,      // 000 001
		PositiveY = 2,      // 000 010
		PositiveZ = 4,      // 000 100
		NegativeX = 8,      // 001 000
		NegativeY = 16,     // 010 000
		NegativeZ = 32,     // 100 000

		X = 9,
		Y = 18,
		Z = 36,

		All = 63,

	} // enum Axis


	public struct AxisUtility
	{
		/// <summary>
		/// 转换为轴对齐的方向向量
		/// 仅当 axis 是单个轴时才返回有效向量, 否则返回零向量
		/// </summary>
		public static Vector3 ToVector(Axis axis)
		{
			switch (axis)
			{
				case Axis.PositiveX: case Axis.X: return new Vector3(1f, 0f, 0f);
				case Axis.NegativeX: return new Vector3(-1f, 0f, 0f);
				case Axis.PositiveY: case Axis.Y: return new Vector3(0f, 1f, 0f);
				case Axis.NegativeY: return new Vector3(0f, -1f, 0f);
				case Axis.PositiveZ: case Axis.Z: return new Vector3(0f, 0f, 1f);
				case Axis.NegativeZ: return new Vector3(0f, 0f, -1f);
				default: return new Vector3();
			}
		}


		/// <summary>
		/// 获取一个轴向的反方向
		/// 仅当 axis 是有效的方向才返回反方向, 否则返回原始方向
		/// </summary>
		public static Axis Reverse(Axis axis)
		{
			switch (axis)
			{
				case Axis.PositiveX: return Axis.NegativeX;
				case Axis.NegativeX: return Axis.PositiveX;
				case Axis.PositiveY: return Axis.NegativeY;
				case Axis.NegativeY: return Axis.PositiveY;
				case Axis.PositiveZ: return Axis.NegativeZ;
				case Axis.NegativeZ: return Axis.PositiveZ;
				default: return axis;
			}
		}


		/// <summary>
		/// 将一个轴向转化为 0~2 的索引
		/// 仅当 axis 是单个轴时才返回有效索引, 否则返回 -1
		/// </summary>
		public static int ToIndex3(Axis axis)
		{
			switch (axis)
			{
				case Axis.PositiveX: case Axis.NegativeX: case Axis.X: return 0;
				case Axis.PositiveY: case Axis.NegativeY: case Axis.Y: return 1;
				case Axis.PositiveZ: case Axis.NegativeZ: case Axis.Z: return 2;
				default: return -1;
			}
		}


		/// <summary>
		/// 将 0~2 的索引转换为轴
		/// </summary>
		public static Axis FromIndex3(int index)
		{
			switch (index)
			{
				case 0: return Axis.X;
				case 1: return Axis.Y;
				case 2: return Axis.Z;
				default: return Axis.None;
			}
		}


		/// <summary>
		/// 将一个轴向转化为 0~5 的索引
		/// 仅当 axis 是有效的方向才返回有效索引, 否则返回 -1
		/// </summary>
		public static int ToIndex6(Axis axis)
		{
			switch (axis)
			{
				case Axis.PositiveX: return 0;
				case Axis.NegativeX: return 1;
				case Axis.PositiveY: return 2;
				case Axis.NegativeY: return 3;
				case Axis.PositiveZ: return 4;
				case Axis.NegativeZ: return 5;
				default: return -1;
			}
		}


		/// <summary>
		/// 将 0~5 的索引转换为方向
		/// </summary>
		public static Axis FromIndex6(int index)
		{
			switch (index)
			{
				case 0: return Axis.PositiveX;
				case 1: return Axis.NegativeX;
				case 2: return Axis.PositiveY;
				case 3: return Axis.NegativeY;
				case 4: return Axis.PositiveZ;
				case 5: return Axis.NegativeZ;
				default: return Axis.None;
			}
		}


		/// <summary>
		/// 返回一个向量最接近的轴向
		/// 向量必须为非零向量，否则返回 Axis.None
		/// </summary>
		public static Axis FromVector(Vector3 vector)
		{
			if (vector == Vector3.zero) return Axis.None;

			Vector3 abs = new Vector3(Mathf.Abs(vector.x), Mathf.Abs(vector.y), Mathf.Abs(vector.z));

			if (abs.x > abs.y)
			{
				if (abs.x > abs.z)
				{
					return vector.x > 0f ? Axis.PositiveX : Axis.NegativeX;
				}
				else
				{
					return vector.z > 0f ? Axis.PositiveZ : Axis.NegativeZ;
				}
			}
			else
			{
				if (abs.y > abs.z)
				{
					return vector.y > 0f ? Axis.PositiveY : Axis.NegativeY;
				}
				else
				{
					return vector.z > 0f ? Axis.PositiveZ : Axis.NegativeZ;
				}
			}
		}
	}

} // namespace WhiteCat