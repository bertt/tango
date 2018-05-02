using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace WhiteCat
{
	/// <summary>
	/// 标记在 float 或 int 字段上, 限制可以设置的最小值
	/// </summary>
	[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
	public sealed class MinAttribute : PropertyAttributeWithEditor
	{

#if UNITY_EDITOR

		double _min;


		public MinAttribute(double min)
		{
			_min = min;
		}


		protected override void Editor_OnGUI(Rect rect, SerializedProperty property, GUIContent label)
		{
			switch (property.propertyType)
			{
				case SerializedPropertyType.Float:
					{
						property.floatValue = Mathf.Max(
							EditorGUI.FloatField(rect, label, property.floatValue),
							(float)_min);
						break;
					}
				case SerializedPropertyType.Integer:
					{
						property.intValue = Mathf.Max(
							EditorGUI.IntField(rect, label, property.intValue),
							(int)_min);
						break;
					}
				default:
					{
						EditorGUI.LabelField(rect, label.text, "Use Min with float or int.");
						break;
					}
			}
		}

#else

		public MinAttribute(double min)
		{
		}

#endif // UNITY_EDITOR

	} // class MinAttribute

} // namespace WhiteCat