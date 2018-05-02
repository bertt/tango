#if UNITY_EDITOR
using UnityEditor;
using WhiteCatEditor;
#endif

namespace WhiteCat
{
	/// <summary>
	/// ScriptableComponentWithEditor
	/// </summary>
	public class ScriptableComponentWithEditor : ScriptableComponent
	{

#if UNITY_EDITOR

		EditorForScriptableComponent _editor;


		/// <summary>
		/// 在实现编辑器方法时用以访问当前 Editor 对象
		/// </summary>
		protected EditorForScriptableComponent editor
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
		/// Editor_RequiresConstantRepaint
		/// </summary>
		protected virtual bool Editor_RequiresConstantRepaint()
		{
			return false;
		}


		/// <summary>
		/// Editor_OnInspectorGUI
		/// </summary>
		protected virtual void Editor_OnInspectorGUI()
		{
			_editor.DrawDefaultInspector();
        }


		/// <summary>
		/// Editor_OnSceneGUI
		/// </summary>
		protected virtual void Editor_OnSceneGUI()
		{
		}


		/// <summary>
		/// EditorForScriptableComponent
		/// </summary>
		[CustomEditor(typeof(ScriptableComponentWithEditor), true)]
		protected class EditorForScriptableComponent : BaseEditor
		{
			void OnEnable()
			{
				var obj = (target as ScriptableComponentWithEditor);

				if (obj._editor != null)
				{
					obj.Editor_OnDisable();
				}

				obj._editor = this;
				obj.Editor_OnEnable();
			}


			void OnDisable()
			{
				var obj = (target as ScriptableComponentWithEditor);

				if (obj._editor == this)
				{
					obj.Editor_OnDisable();
					obj._editor = null;
				}
			}


			public override bool RequiresConstantRepaint()
			{
				var obj = (target as ScriptableComponentWithEditor);
				
				if (obj._editor == null)
				{
					obj._editor = this;
					obj.Editor_OnEnable();
				}

				return obj.Editor_RequiresConstantRepaint();
			}


			public override void OnInspectorGUI()
			{
				var obj = (target as ScriptableComponentWithEditor);

				if (obj._editor == null)
				{
					obj._editor = this;
					obj.Editor_OnEnable();
				}

				obj.Editor_OnInspectorGUI();
			}


			void OnSceneGUI()
			{
				var obj = (target as ScriptableComponentWithEditor);

				if (obj._editor == null)
				{
					obj._editor = this;
					obj.Editor_OnEnable();
				}

				obj.Editor_OnSceneGUI();
			}

		} // class EditorForScriptableComponent

#endif // UNITY_EDITOR

	} // class ScriptableComponentWithEditor

} // namespace WhiteCat