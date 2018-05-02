using System;

namespace WhiteCat
{
	/// <summary>
	/// 范围 (一维, 包含最小值而不包含最大值)
	/// </summary>
	[Serializable]
	public struct Range
	{
		/// <summary>
		/// 最小值 (包含)
		/// </summary>
		public float min;


		/// <summary>
		/// 最大值 (不包含)
		/// </summary>
		public float max;


		/// <summary>
		/// 根据最小最大值构造范围
		/// </summary>
		public Range(float min, float max)
		{
			this.min = min;
			this.max = max;
		}


		/// <summary>
		/// 大小
		/// 修改大小时维持中心不变
		/// </summary>
		public float size
		{
			get { return max - min; }
			set
			{
				float halfExtend = (value - max + min) * 0.5f;
				min -= halfExtend;
				max += halfExtend;
			}
		}


		/// <summary>
		/// 中心
		/// 修改中心时维持大小不变
		/// </summary>
		public float center
		{
			get { return (min + max) * 0.5f; }
			set
			{
				float halfSize = (max - min) * 0.5f;
				min = value - halfSize;
				max = value + halfSize;
			}
		}


		/// <summary>
		/// 确保最小最大值关系正确
		/// </summary>
		public void OrderMinMax()
		{
			if (min > max) Kit.Swap(ref min, ref max);
		}


		/// <summary>
		/// 判断是否包含某个值
		/// </summary>
		public bool Contains(float value)
		{
			return value >= min && value < max;
		}


		/// <summary>
		/// 判断是否与另一范围有交集
		/// </summary>
		public bool Overlap(Range other)
		{
			return min < other.max && max >= other.min;
		}


		/// <summary>
		/// 获取两个范围的交集
		/// </summary>
		public Range GetOverlappedRange(Range other)
		{
			if (min > other.min) other.min = min;
			if (max < other.max) other.max = max;
			return other;
		}

	} // struct Range

} // namespace WhiteCat