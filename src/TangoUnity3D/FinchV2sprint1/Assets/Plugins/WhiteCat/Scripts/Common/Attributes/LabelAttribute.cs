using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace WhiteCat
{
	/// <summary>
	/// 标记在一个字段上, 可以指定其显示的 Label
	/// </summary>
	[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
	public sealed class LabelAttribute : PropertyAttributeWithEditor
	{

#if UNITY_EDITOR

		GUIContent _label;


		public LabelAttribute(string label)
		{
			_label = new GUIContent(label);
		}


		protected override void Editor_OnGUI(Rect rect, SerializedProperty property, GUIContent label)
		{
			EditorGUI.PropertyField(rect, property, _label, true);
		}

#else

		public LabelAttribute(string label)
		{
		}

#endif // UNITY_EDITOR

	} // class LabelAttribute

} // namespace WhiteCat