using System;

namespace WhiteCat.Paths
{
	/// <summary>
	/// 路径位置
	/// </summary>
	[Serializable]
	public struct Location
	{
		public int index;       // 路径段索引
		public float time;      // 路径段时间 (三次样条参数 t)


		public Location(int index, float time)
		{
			this.index = index;
			this.time = time;
		}


		public void Set(int index, float time)
		{
			this.index = index;
			this.time = time;
		}

	} // struct Location

} // namespace WhiteCat.Paths