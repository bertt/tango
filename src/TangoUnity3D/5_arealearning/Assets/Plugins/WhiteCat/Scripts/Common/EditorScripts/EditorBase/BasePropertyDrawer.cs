#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

namespace WhiteCatEditor
{
	/// <summary>
	/// BasePropertyDrawer
	/// </summary>
	public class BasePropertyDrawer : PropertyDrawer
	{
		/// <summary>
		/// 获取字段的值, 在 GetPropertyHeight 和 OnGUI 中使用
		/// </summary>
		public T GetFieldValue<T>(SerializedProperty property)
		{
			return (T)fieldInfo.GetValue(property.serializedObject.targetObject);
		}


		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return EditorGUI.GetPropertyHeight(property, label, true);
		}


		public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
		{
			EditorGUI.PropertyField(rect, property, label, true);
		}

	} // class BasePropertyDrawer

} // namespace WhiteCatEditor

#endif // UNITY_EDITOR