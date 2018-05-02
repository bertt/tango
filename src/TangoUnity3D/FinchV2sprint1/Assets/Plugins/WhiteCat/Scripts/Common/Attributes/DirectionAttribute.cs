using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace WhiteCat
{
	/// <summary>
	/// 标记在 Vector3 字段上, 可以让它代表方向向量, Inspector 上显示的是 XY 轴的欧拉角
	/// </summary>
	[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
	public sealed class DirectionAttribute : PropertyAttributeWithEditor
	{

#if UNITY_EDITOR

		float _length;
		Vector3 _direction;
		Vector2 _eulerAngles;


		public DirectionAttribute(float length = 1f)
		{
			_length = length;
			_direction = new Vector3(0, 0, length);
			_eulerAngles = Vector2.zero;
		}


		protected override float Editor_GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return EditorGUIUtility.singleLineHeight;
		}


		protected override void Editor_OnGUI(Rect rect, SerializedProperty property, GUIContent label)
		{
			if (property.propertyType == SerializedPropertyType.Vector3)
			{
				if (_direction != property.vector3Value)
				{
					_direction = property.vector3Value;
					_eulerAngles = Quaternion.LookRotation(_direction).eulerAngles;
				}

				EditorGUI.BeginChangeCheck();
				_eulerAngles = EditorGUI.Vector2Field(rect, label, _eulerAngles);
				if (EditorGUI.EndChangeCheck())
				{
					property.vector3Value = _direction = Quaternion.Euler(_eulerAngles) * new Vector3(0, 0, _length);
				}
			}
			else
			{
				EditorGUI.LabelField(rect, label.text, "Use Direction with Vector3.");
			}
		}

#else

		public DirectionAttribute(float length = 1f)
		{
        }

#endif // UNITY_EDITOR

	} // class DirectionAttribute

} // namespace WhiteCat