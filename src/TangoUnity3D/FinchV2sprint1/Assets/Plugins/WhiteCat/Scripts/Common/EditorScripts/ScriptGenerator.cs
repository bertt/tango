#if UNITY_EDITOR

using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using WhiteCat;
using UnityEditor;

namespace WhiteCatEditor
{
	class ScriptGenerater
	{
		static void CreateScript(string className, Action<StreamWriter> write)
		{
			string path = string.Format("{0}/{1}.cs", EditorKit.activeDirectory, className);

			using (var stream = new FileStream(path, FileMode.Create, FileAccess.Write))
			{
				using (var writer = new StreamWriter(stream))
				{
					write(writer);
				}
			}

			AssetDatabase.Refresh();
			Selection.activeObject = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
		}


		[MenuItem("Assets/Create/White Cat/Layers Script")]
		static void CreateLayerScript()
		{
			CreateScript("Layers", writer =>
			{
				writer.Write(
@"
namespace WhiteCat
{
	/// <summary> Constants of Layers. </summary>
	public struct Layers
	{"				);

				List<string> list = new List<string>(32);

				for (int i = 0; i < 32; i++)
				{
					var name = LayerMask.LayerToName(i);

					writer.Write(string.Format(
		@"
		/// <summary> Layer {0}, Name: {1} </summary>
		public const int {2} = {0};
"						, i, string.IsNullOrEmpty(name) ? "(none)" : name, GetVariableName(name, "Layer"+i, list)));
				}

				writer.Write(
		@"

		/// <summary> Constants of LayerMasks. </summary>
		public struct Masks
		{"			);

				for (int i = 0; i < 32; i++)
				{
					var name = LayerMask.LayerToName(i);

					writer.Write(string.Format(
			@"
			/// <summary> Mask of Layer {0}, Name: {1} </summary>
			public const int {2} = {3};
"						, i, string.IsNullOrEmpty(name) ? "(none)" : name, list[i], 1 << i));
				}

				writer.Write(
@"		}
	}
}"					);
			});
		}


		[MenuItem("Assets/Create/White Cat/Scenes Script (File Name)")]
		static void CreateSceneScriptFileName()
		{
			CreateSceneScript(0);
		}


		[MenuItem("Assets/Create/White Cat/Scenes Script (Folder + File Name)")]
		static void CreateSceneScriptFolderFileName()
		{
			CreateSceneScript(1);
		}


		// 创建场景脚本, depth 指生成变量名时, 从场景文件开始向上保留的文件夹数量
		static void CreateSceneScript(int depth)
		{
			CreateScript("Scenes", writer =>
			{
				writer.Write(
@"
namespace WhiteCat
{
	/// <summary> Constants of Scenes. </summary>
	public struct Scenes
	{"				);

				var scenes = EditorBuildSettings.scenes;
				List<string> list = new List<string>(scenes.Length);

				for (int i = 0; i < scenes.Length; i++)
				{
					var name = GetSubpath(scenes[i].path, depth, true);

					writer.Write(string.Format(
		@"
		/// <summary> Scene {0}, Path: {1} </summary>
		public const int {2} = {0};
"						, i, scenes[i].path, GetVariableName(name, "Scene"+i, list)));
				}

				writer.Write(
@"	}
}"					);
			});
		}


		// 字符串转化为有效的变量名
		// 仅支持下划线，英文字母 和 数字，其他字符被忽略, 并且在 list 中保持唯一
		// 如果无法获取有效的变量名，则使用 fallbackName 作为变量名
		static string GetVariableName(string originalName, string fallbackName, List<string> list)
		{
			var chars = new List<char>(originalName);
			bool invalid;
			bool upper = true;

			for (int i = 0; i < chars.Count; i++)
			{
				var c = chars[i];

				if (i == 0) invalid = (c != '_' && !Kit.IsLowerOrUpper(c));
				else invalid = (c != '_' && !Kit.IsDigit(c) && !Kit.IsLowerOrUpper(c));

				if (invalid)
				{
					chars.RemoveAt(i--);
					upper = true;
				}
				else if (upper)
				{
					if (Kit.IsLower(c))
					{
						chars[i] = (char)(c + 'A' - 'a');
					}
					upper = false;
				}
			}

			originalName = new string(chars.ToArray());
			if (originalName.Length == 0 || list.Contains(originalName))
			{
				originalName = fallbackName;
			}

			list.Add(originalName);
			return originalName;
		}


		// 将一个有效路径从尾部开始向前保留限定的层级
		static string GetSubpath(string path, int depth, bool removeExtension)
		{
			if (removeExtension)
			{
				path = path.Substring(0, path.LastIndexOf('.'));
			}

			for (int i = path.Length - 1; i >= 0; i--)
			{
				if (path[i] == '/')
				{
					if (depth <= 0) return path.Substring(i + 1);
					else depth--;
				}
			}

			return path;
		}

	} // class ScriptGenerater

} // namespace WhiteCatEditor

#endif // UNITY_EDITOR