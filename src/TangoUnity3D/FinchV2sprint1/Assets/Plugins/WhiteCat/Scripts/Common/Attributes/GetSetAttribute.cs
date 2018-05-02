using System;
using System.Reflection;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace WhiteCat
{
	/// <summary>
	/// 标记在一个字段上, 指定一个 property 来代理读取和设置值的过程
	/// 注意, 该 property 仅可修改 target 的数据. 如果要修改其他对象的数据, 请自定义编辑器
	/// </summary>
	[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
	public sealed class GetSetAttribute : PropertyAttributeWithEditor
	{

#if UNITY_EDITOR

		string _propertyName;
		string _undoString;
		UnityEngine.Object _target;
		PropertyInfo _propertyInfo;


		public GetSetAttribute(string propertyName)
		{
			_propertyName = propertyName;
		}


		protected override void Editor_OnGUI(Rect rect, SerializedProperty property, GUIContent label)
		{
			if (_target == null)
			{
				_target = property.serializedObject.targetObject;
				_undoString = _target.ToString();
				_propertyInfo = Kit.GetPropertyInfo(_target, _propertyName);

				if (_propertyInfo == null)
				{
					EditorGUI.LabelField(rect, label.text, "Can not find property.");
				}
				else if (drawer.fieldInfo.FieldType != _propertyInfo.PropertyType)
				{
					_propertyInfo = null;
					EditorGUI.LabelField(rect, label.text, "Property type is not same as field.");
				}
				else if (!_propertyInfo.CanRead || !_propertyInfo.CanWrite)
				{
					_propertyInfo = null;
					EditorGUI.LabelField(rect, label.text, "Property can not read or write.");
				}
			}

			if (_propertyInfo == null) return;

			EditorGUI.BeginChangeCheck();
			object value = _propertyInfo.GetValue(_target, null);

			switch (property.propertyType)
			{
				case SerializedPropertyType.AnimationCurve:
					{
						value = EditorGUI.CurveField(rect, label, (AnimationCurve)value);
						break;
					}
				case SerializedPropertyType.Boolean:
					{
						value = EditorGUI.Toggle(rect, label, (bool)value);
						break;
					}
				case SerializedPropertyType.Bounds:
					{
						value = EditorGUI.BoundsField(rect, label, (Bounds)value);
						break;
					}
				case SerializedPropertyType.Color:
					{
						value = EditorGUI.ColorField(rect, label, (Color)value);
						break;
					}
				case SerializedPropertyType.Enum:
					{
						value = EditorGUI.EnumPopup(rect, label, (Enum)value);
						break;
					}
				case SerializedPropertyType.Float:
					{
						value = EditorGUI.FloatField(rect, label, (float)value);
						break;
					}
				case SerializedPropertyType.Integer:
					{
						value = EditorGUI.IntField(rect, label, (int)value);
						break;
					}
				case SerializedPropertyType.ObjectReference:
					{
						value = EditorGUI.ObjectField(
							rect,
							label,
							value as UnityEngine.Object,
							_propertyInfo.PropertyType,
							!EditorUtility.IsPersistent(_target));
						break;
					}
				case SerializedPropertyType.Rect:
					{
						value = EditorGUI.RectField(rect, label, (Rect)value);
						break;
					}
				case SerializedPropertyType.String:
					{
						value = EditorGUI.TextField(rect, label, (string)value);
						break;
					}
				case SerializedPropertyType.Vector2:
					{
						value = EditorGUI.Vector2Field(rect, label, (Vector2)value);
						break;
					}
				case SerializedPropertyType.Vector3:
					{
						value = EditorGUI.Vector3Field(rect, label, (Vector3)value);
						break;
					}
				case SerializedPropertyType.Vector4:
					{
						value = EditorGUI.Vector4Field(rect, label.text, (Vector4)value);
						break;
					}
				default:
					{
						EditorGUI.LabelField(rect, label.text, "Type is not supported.");
						break;
					}
			}

			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(_target, _undoString);
				_propertyInfo.SetValue(_target, value, null);
				EditorUtility.SetDirty(_target);
			}
		}

#else

		public GetSetAttribute(string propertyName)
		{
		}

#endif // UNITY_EDITOR

	} // class GetSetAttribute

} // namespace WhiteCat