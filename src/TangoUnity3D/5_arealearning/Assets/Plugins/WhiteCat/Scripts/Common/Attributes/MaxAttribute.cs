using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace WhiteCat
{
	/// <summary>
	/// 标记在 float 或 int 字段上, 限制可以设置的最大值
	/// </summary>
	[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
	public sealed class MaxAttribute : PropertyAttributeWithEditor
	{

#if UNITY_EDITOR

		double _max;


		public MaxAttribute(double max)
		{
			_max = max;
		}


		protected override void Editor_OnGUI(Rect rect, SerializedProperty property, GUIContent label)
		{
			switch (property.propertyType)
			{
				case SerializedPropertyType.Float:
					{
						property.floatValue = Mathf.Min(
							EditorGUI.FloatField(rect, label, property.floatValue),
							(float)_max);
						break;
					}
				case SerializedPropertyType.Integer:
					{
						property.intValue = Mathf.Min(
							EditorGUI.IntField(rect, label, property.intValue),
							(int)_max);
						break;
					}
				default:
					{
						EditorGUI.LabelField(rect, label.text, "Use Max with float or int.");
						break;
					}
			}
		}

#else

		public MaxAttribute(double max)
		{
		}

#endif // UNITY_EDITOR

	} // class MaxAttribute

} // namespace WhiteCat