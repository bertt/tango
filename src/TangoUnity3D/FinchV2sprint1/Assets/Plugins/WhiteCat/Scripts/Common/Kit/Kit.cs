using System.Collections;

namespace WhiteCat
{
	/// <summary>
	/// 常用方法
	/// </summary>
	public partial struct Kit
	{
		/// <summary>
		/// 全局 Random 对象, 用以执行常规随机任务
		/// </summary>
		public static readonly Random random = new Random();


		/// <summary>
		/// 交换两个变量的值
		/// </summary>
		public static void Swap<T>(ref T a, ref T b)
		{
			T c = a;
			a = b;
			b = c;
		}


		/// <summary>
		/// 判断集合是否为 null 或元素个数是否为 0
		/// </summary>
		public static bool IsNullOrEmpty(ICollection collection)
		{
			return collection == null || collection.Count == 0;
		}


		/// <summary>
		/// 判断一个字符是否为阿拉伯数字字符
		/// </summary>
		public static bool IsDigit(char c)
		{
			return c >= '0' && c <= '9';
		}


		/// <summary>
		/// 判断一个字符是否为英文小写字母
		/// </summary>
		public static bool IsLower(char c)
		{
			return c >= 'a' && c <= 'z';
		}


		/// <summary>
		/// 判断一个字符是否为英文大写字母
		/// </summary>
		public static bool IsUpper(char c)
		{
			return c >= 'A' && c <= 'Z';
		}


		/// <summary>
		/// 判断一个字符是否为英文字母
		/// </summary>
		public static bool IsLowerOrUpper(char c)
		{
			return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');
		}

	} // struct Kit

} // namespace WhiteCat