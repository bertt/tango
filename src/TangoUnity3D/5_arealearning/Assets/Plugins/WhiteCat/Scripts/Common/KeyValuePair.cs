
namespace WhiteCat
{
	/// <summary>
	/// 可读写值的键值对. 一般不使用无参构造, 因为 key 是只读的
	/// </summary>
	public struct KeyValuePair<TKey, TValue>
	{
		TKey _key;


		public TValue value;


		public TKey key
		{
			get { return _key; }
		}


		public KeyValuePair(TKey key)
		{
			this._key = key;
			value = default(TValue);
		}


		public KeyValuePair(TKey key, TValue value)
		{
			this._key = key;
			this.value = value;
		}

	} // struct KeyValuePair

} // namespace WhiteCat