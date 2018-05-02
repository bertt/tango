#pragma warning disable 0414

using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace WhiteCat
{
	/// <summary>
	/// 操作收集器
	/// </summary>
	[AddComponentMenu("White Cat/Common/Action Collector")]
	public class ActionCollector : ScriptableComponentWithEditor
	{
		[SerializeField] UnityEvent _action = new UnityEvent();


		public event UnityAction action
		{
			add { _action.AddListener(value); }
			remove { _action.RemoveListener(value); }
		}


		public void Invoke()
		{
			_action.Invoke();
		}

#if UNITY_EDITOR

		[SerializeField] string _comment = "Comment...";
		SerializedProperty _actionProperty;
		SerializedProperty _commentProperty;


		protected override void Editor_OnEnable()
		{
			_actionProperty = editor.serializedObject.FindProperty("_action");
			_commentProperty = editor.serializedObject.FindProperty("_comment");
		}


		protected override void Editor_OnDisable()
		{
			_actionProperty = null;
			_commentProperty = null;
		}


		protected override void Editor_OnInspectorGUI()
		{
			Rect rect = EditorGUILayout.GetControlRect(true, 18f);
			float xMax = rect.xMax;
			rect.width = 72f;

			if (GUI.Button(rect, "Invoke"))
			{
				Invoke();
			}

			rect.x = rect.xMax + 6f;
			rect.xMax = xMax;

			editor.serializedObject.Update();

			EditorGUI.PropertyField(rect, _commentProperty, GUIContent.none);
			EditorGUILayout.PropertyField(_actionProperty);

			editor.serializedObject.ApplyModifiedProperties();
		}

#endif // UNITY_EDITOR

	} // class ActionCollector

} // namespace WhiteCat