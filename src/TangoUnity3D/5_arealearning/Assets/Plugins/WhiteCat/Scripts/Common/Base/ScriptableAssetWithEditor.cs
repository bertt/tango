#if UNITY_EDITOR
using UnityEditor;
using WhiteCatEditor;
#endif

namespace WhiteCat
{
	/// <summary>
	/// ScriptableAssetWithEditor
	/// </summary>
	public class ScriptableAssetWithEditor : ScriptableAsset
	{

#if UNITY_EDITOR

		EditorForScriptableAsset _editor;


		/// <summary>
		/// 在实现编辑器方法时用以访问当前 Editor 对象
		/// </summary>
		protected EditorForScriptableAsset editor
		{
			get { return _editor; }
		}


		/// <summary>
		/// Editor_OnEnable
		/// </summary>
		protected virtual void Editor_OnEnable()
		{
		}


		/// <summary>
		/// Editor_OnDisable
		/// </summary>
		protected virtual void Editor_OnDisable()
		{
		}


		/// <summary>
		/// Editor_OnInspectorGUI
		/// </summary>
		protected virtual void Editor_OnInspectorGUI()
		{
			_editor.DrawDefaultInspector();
		}


		/// <summary>
		/// EditorForScriptableAsset
		/// </summary>
		[CustomEditor(typeof(ScriptableAssetWithEditor), true)]
		protected class EditorForScriptableAsset : BaseEditor
		{
			void OnEnable()
			{
				var obj = (target as ScriptableAssetWithEditor);

				if (obj._editor != null)
				{
					obj.Editor_OnDisable();
				}

				obj._editor = this;
				obj.Editor_OnEnable();
			}


			void OnDisable()
			{
				var obj = (target as ScriptableAssetWithEditor);

				if (obj._editor == this)
				{
					obj.Editor_OnDisable();
					obj._editor = null;
				}
			}


			public override void OnInspectorGUI()
			{
				var obj = (target as ScriptableAssetWithEditor);

				if (obj._editor == null)
				{
					obj._editor = this;
					obj.Editor_OnEnable();
				}

				obj.Editor_OnInspectorGUI();
			}

		} // class EditorForScriptableAsset

#endif // UNITY_EDITOR

	} // class ScriptableAssetWithEditor

} // namespace WhiteCat