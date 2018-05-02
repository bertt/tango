using System;
using System.Collections.Generic;

namespace WhiteCat
{
	/// <summary>
	/// 随机数
	/// </summary>
	[Serializable]
	public class Random
	{
		/// <summary>
		/// 种子
		/// </summary>
		public uint seed;


		// 全局 Random 实例
		static readonly Random _global = new Random((uint)(DateTime.Now.Ticks & 0xFFFFFFFF));


		/// <summary>
		/// 默认构造方法, 连续调用时不会产生相同的结果
		/// </summary>
		public Random()
		{
			_global.Range01();
			seed = ~_global.seed;
		}


		/// <summary>
		/// 指定种子的构造方法
		/// </summary>
		/// <param name="seed"> 指定的种子, 相同的种子会产生相同的随机序列 </param>
		public Random(uint seed)
		{
			this.seed = seed;
		}


		/// <summary>
		/// 返回一个 [0, 1) 范围内的随机浮点数
		/// </summary>
		public double Range01()
		{
			//
			// https://en.wikipedia.org/wiki/Lehmer_random_number_generator
			//
			seed = (uint)((seed % 2147483646U + 1U) * 48271UL % 2147483647UL) - 1U;

			return seed / 2147483646.0;
		}


		/// <summary>
		/// 返回一个指定范围内的随机浮点数
		/// </summary>
		/// <param name="minValue"> 返回的随机数的下界(包含) </param>
		/// <param name="maxValue"> 返回的随机数的上界(不包含) </param>
		/// <returns> [minValue, maxValue) 范围的均匀分布随机数 </returns>
		public float Range(float minValue, float maxValue)
		{
			if (minValue > maxValue) Kit.Swap(ref minValue, ref maxValue);
			return minValue + (maxValue - minValue) * (float)Range01();
		}


		/// <summary>
		/// 返回一个指定范围内的随机整数
		/// </summary>
		/// <param name="minValue"> 返回的随机数的下界(包含) </param>
		/// <param name="maxValue"> 返回的随机数的上界(不包含) </param>
		/// <returns> [minValue, maxValue) 范围的均匀分布随机数 </returns>
		public int Range(int minValue, int maxValue)
		{
			if (minValue > maxValue) Kit.Swap(ref minValue, ref maxValue);
			return minValue + (int)((maxValue - minValue) * Range01());
		}


		/// <summary>
		/// 测试随机事件在一次独立实验中是否发生
		/// </summary>
		/// <param name="probability"> [0f, 1f] 范围的概率 </param>
		/// <returns> 如果事件发生返回 true, 否则返回 false </returns>
		public bool Test(float probability)
		{
			return Range01() < probability;
		}


		/// <summary>
		/// 从一组元素中选择一个. 如果所有元素被选中概率之和小于 1, 那么最后一个元素被选中概率相应增加
		/// </summary>
		/// <param name="getProbability"> 每个元素被选中的概率 </param>
		/// <param name="startIndex"> 开始遍历的索引 </param>
		/// <param name="count"> 遍历元素的数量 </param>
		/// <returns> 被选中的元素索引 </returns>
		public int Choose(Func<int, float> getProbability, int startIndex, int count)
		{
			int lastIndex = startIndex + count - 1;
			float rest = (float)Range01();
			float current;

			for (; startIndex < lastIndex; startIndex++)
			{
				current = getProbability(startIndex);
				if (rest < current) return startIndex;
				else rest -= current;
			}

			return lastIndex;
		}


		/// <summary>
		/// 从一组元素中选择一个. 如果所有元素被选中概率之和小于 1, 那么最后一个元素被选中概率相应增加
		/// </summary>
		/// <param name="probabilities"> 每个元素被选中的概率 </param>
		/// <param name="startIndex"> 开始遍历的索引 </param>
		/// <param name="count"> 遍历元素的数量. 如果这个值无效, 自动遍历到列表尾部 </param>
		/// <returns> 被选中的元素索引 </returns>
		public int Choose(IList<float> probabilities, int startIndex = 0, int count = 0)
		{
			if (count < 1 || count > probabilities.Count - startIndex)
			{
				count = probabilities.Count - startIndex;
			}
			return Choose(i => probabilities[i], startIndex, count);
		}


		/// <summary>
		/// 将一组元素随机排序
		/// </summary>
		/// <typeparam name="T"> 元素类型 </typeparam>
		/// <param name="list"> 元素列表 </param>
		/// <param name="startIndex"> 开始排序的索引 </param>
		/// <param name="count"> 执行排序的元素总数. 如果这个值无效, 自动遍历到列表尾部 </param>
		public void Sort<T>(IList<T> list, int startIndex = 0, int count = 0)
		{
			int lastIndex = startIndex + count;
			if (lastIndex <= startIndex || lastIndex > list.Count)
			{
				lastIndex = list.Count;
			}

			lastIndex -= 1;

			T temp;
			int swapIndex;

			for (int i = startIndex; i < lastIndex; i++)
			{
				swapIndex = Range(i, lastIndex + 1);
				temp = list[i];
				list[i] = list[swapIndex];
				list[swapIndex] = temp;
			}
		}


		/// <summary>
		/// 产生正态分布的随机数
		/// 正态分布随机数落在 μ±σ, μ±2σ, μ±3σ 的概率依次为 68.26%, 95.44%, 99.74%
		/// </summary>
		/// <param name="averageValue"> 正态分布的平均值, 即 N(μ, σ^2) 中的 μ </param>
		/// <param name="standardDeviation"> 正态分布的标准差, 即 N(μ, σ^2) 中的 σ </param>
		/// <returns> 返回正态分布的随机数. 理论值域是 μ±∞ </returns>
		public float Normal(float averageValue, float standardDeviation)
		{
			//
			// https://en.wikipedia.org/wiki/Box-Muller_transform
			//
			return averageValue + standardDeviation * (float)
				(
					Math.Sqrt(-2.0 * Math.Log(1.0 - Range01()))
					* Math.Sin((Math.PI + Math.PI) * Range01())
				);
		}

	} // class Random

} // namespace WhiteCat