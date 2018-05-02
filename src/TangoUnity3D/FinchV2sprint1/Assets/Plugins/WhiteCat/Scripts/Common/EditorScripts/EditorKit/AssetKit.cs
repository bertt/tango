#if UNITY_EDITOR

using System.IO;
using System.Collections.Generic;
using UnityEditor;
using WhiteCat;
using UnityObject = UnityEngine.Object;

namespace WhiteCatEditor
{
	/// <summary>
	/// 资源相关的编辑器方法
	/// </summary>
	public partial struct EditorKit
	{
		/// <summary>
		/// 获取激活的目录, 如果没有文件夹或文件被选中, 返回 Assets 目录
		/// </summary>
		public static string activeDirectory
		{
			get
			{
				var objects = Selection.GetFiltered(typeof(UnityObject), SelectionMode.Assets);

				if (!Kit.IsNullOrEmpty(objects))
				{
					string path;

					// 优先选择文件夹
					foreach(var obj in objects)
					{
						path = AssetDatabase.GetAssetPath(obj);
						if (Directory.Exists(path)) return path;
					}

					path = AssetDatabase.GetAssetPath(objects[0]);
					return path.Substring(0, path.LastIndexOf('/'));
				}

				return "Assets";
			}
		}


		/// <summary>
		/// 创建资源文件
		/// </summary>
		/// <typeparam name="T"> 用于创建资源的对象类型 </typeparam>
		/// <param name="unityObject"> 用于创建资源的对象 </param>
		/// <param name="assetPath">
		/// 相对于 Assets 目录的文件路径, 比如: "Assets/MyFolder/MyAsset.asset"
		/// </param>
		/// <param name="autoRename"> 如果存在重名文件是否自动重命名, 如果不自动重命名那么旧文件会被覆盖 </param>
		/// <param name="autoCreateDirectory"> 是否自动创建不存在的目录 </param>
		public static void CreateAsset<T>(
			T unityObject,
			string assetPath,
			bool autoRename = true,
			bool autoCreateDirectory = true)
			where T : UnityObject
		{
			if (autoCreateDirectory)
			{
				int index = assetPath.LastIndexOf('/');
				if (index >= 0)
				{
					Directory.CreateDirectory(assetPath.Substring(0, index));
				}
			}

			if (autoRename)
			{
				assetPath = AssetDatabase.GenerateUniqueAssetPath(assetPath);
			}

			AssetDatabase.CreateAsset(unityObject, assetPath);
		}


		/// <summary>
		/// 创建资源文件, 文件名根据选中的路径和对象类型自动产生
		/// </summary>
		/// <typeparam name="T"> 用于创建资源的对象类型 </typeparam>
		/// <param name="unityObject"> 用于创建资源的对象 </param>
		public static void QuickCreateAsset<T>(T unityObject)
			where T : UnityObject
		{
			string typeName = typeof(T).Name;
			string name = "New" + typeName + ".asset";
			CreateAsset(unityObject, activeDirectory + '/' + name, true, false);
		}


		/// <summary>
		/// 查找指定类型的单个资源
		/// </summary>
		public static T FindAsset<T>() where T : UnityObject
		{
			var guids = AssetDatabase.FindAssets("t:" + typeof(T).FullName);

			if (!Kit.IsNullOrEmpty(guids))
			{
				return AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guids[0]));
			}

			return null;
		}


		/// <summary>
		/// 查找指定类型的所有孤立资源, 孤立资源指单个文件对应单个对象的资源
		/// </summary>
		public static List<T> FindIsolatedAssets<T>() where T : UnityObject
		{
			var guids = AssetDatabase.FindAssets("t:" + typeof(T).FullName);
			List<T> results = new List<T>();

			foreach(var guid in guids)
			{
				results.Add(AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guid)));
			}

			return results;
		}


		/// <summary>
		/// 查找指定类型的所有组合资源, 组合资源指单个文件可能对应多个对象的资源
		/// 结果也包含孤立资源
		/// </summary>
		public static List<T> FindCombinedAssets<T>() where T : UnityObject
		{
			var guids = AssetDatabase.FindAssets("t:" + typeof(T).FullName);
			List<T> results = new List<T>();

			foreach (var guid in guids)
			{
				foreach(var asset in AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GUIDToAssetPath(guid)))
				{
					if (asset is T) results.Add(asset as T);
				}
			}

			return results;
		}

	} // struct EditorKit

} // namespace WhiteCatEditor

#endif // UNITY_EDITOR