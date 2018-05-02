using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace WhiteCat
{
	/// <summary>
	/// 标记在四元数字段上, 将其显示为欧拉角
	/// </summary>
	[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
	public sealed class EulerAnglesAttribute : PropertyAttributeWithEditor
	{

#if UNITY_EDITOR

		Quaternion _quaternion = Quaternion.identity;
		Vector3 _eulerAngles = Vector3.zero;


		protected override float Editor_GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			if (EditorGUIUtility.wideMode)
			{
				return EditorGUIUtility.singleLineHeight;
			}
			else
			{
				return EditorGUIUtility.singleLineHeight * 2;
			}
		}


		protected override void Editor_OnGUI(Rect rect, SerializedProperty property, GUIContent label)
		{
			if (property.propertyType == SerializedPropertyType.Quaternion)
			{
				if (_quaternion != property.quaternionValue)
				{
					_quaternion = property.quaternionValue;
					_eulerAngles = _quaternion.eulerAngles;
				}

				EditorGUI.BeginChangeCheck();
				_eulerAngles = EditorGUI.Vector3Field(rect, label, _eulerAngles);
				if (EditorGUI.EndChangeCheck())
				{
					property.quaternionValue = _quaternion = Quaternion.Euler(_eulerAngles);
				}
			}
			else
			{
				EditorGUI.LabelField(rect, label.text, "Use EulerAngles with Quaternion.");
			}
		}

#endif // UNITY_EDITOR

	} // class EulerAnglesAttribute

} // namespace WhiteCat