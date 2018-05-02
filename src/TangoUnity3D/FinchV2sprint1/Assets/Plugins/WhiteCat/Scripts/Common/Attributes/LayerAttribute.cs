using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace WhiteCat
{
	/// <summary>
	/// 标记在一个 int 字段上, 使其作为 Layer 显示
	/// </summary>
	[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
	public sealed class LayerAttribute : PropertyAttributeWithEditor
	{

#if UNITY_EDITOR

		protected override void Editor_OnGUI(Rect rect, SerializedProperty property, GUIContent label)
		{
			if (property.propertyType == SerializedPropertyType.Integer)
			{
				property.intValue = EditorGUI.LayerField(rect, label, property.intValue);
			}
			else EditorGUI.LabelField(rect, label.text, "Use Layer with int.");
		}

#endif // UNITY_EDITOR

	} // LayerAttribute

} // namespace WhiteCat