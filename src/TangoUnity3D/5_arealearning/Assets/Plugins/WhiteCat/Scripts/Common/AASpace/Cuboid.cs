using System;

namespace WhiteCat
{
	/// <summary>
	/// 长方体 (三维, 包含最小值而不包含最大值)
	/// </summary>
	[Serializable]
	public struct Cuboid
	{
		public Range x;
		public Range y;
		public Range z;

	} // struct Cuboid

} // namespace WhiteCat