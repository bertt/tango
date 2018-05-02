
namespace WhiteCat
{
	/// <summary>
	/// ScriptableAssetSingletonWithEditor
	/// 
	/// 在编辑器和运行时提供访问实例的方法, 不检查资源数量.
	/// 在编辑器, 创建资源后即可访问实例.
	/// 在运行时, 没有被场景引用, 或不在 Resources 里的资源在 Build 时会被忽略,
	/// 所以在运行时需要通过场景引用, 或 Resources 加载的方式来创建实例.
	/// </summary>
	public class ScriptableAssetSingletonWithEditor<T> : ScriptableAssetWithEditor
		where T : ScriptableAssetSingletonWithEditor<T>
	{
		static T _instance;


		/// <summary>
		/// 访问资源的实例
		/// </summary>
		public static T instance
		{
			get
			{
#if UNITY_EDITOR
				if (_instance == null)
				{
					_instance = WhiteCatEditor.EditorKit.FindAsset<T>();
				}
#endif
				return _instance;
			}
		}


		protected ScriptableAssetSingletonWithEditor()
		{
			_instance = this as T;
		}

	} // class ScriptableAssetSingletonWithEditor

} // namespace WhiteCat