using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace WhiteCat
{
	/// <summary>
	/// 标记在 float 或 int 字段上, 可以限制它的取值范围
	/// </summary>
	[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
	public sealed class ClampAttribute : PropertyAttributeWithEditor
    {

#if UNITY_EDITOR

		double _min, _max;


		public ClampAttribute(double min, double max)
		{
			_min = min;
			_max = max;
		}


        protected override void Editor_OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.Float:
                    {
                        property.floatValue = Mathf.Clamp(
                            EditorGUI.FloatField(rect, label, property.floatValue),
                            (float)_min,
                            (float)_max);
                        break;
                    }
                case SerializedPropertyType.Integer:
                    {
                        property.intValue = Mathf.Clamp(
                            EditorGUI.IntField(rect, label, property.intValue),
                            (int)_min,
                            (int)_max);
                        break;
                    }
                default:
                    {
                        EditorGUI.LabelField(rect, label.text, "Use Clamp with float or int.");
                        break;
                    }
            }
        }

#else

        public ClampAttribute(double min, double max)
		{
        }

#endif // UNITY_EDITOR

    } // class ClampAttribute

} // namespace WhiteCat