#if UNITY_EDITOR

#if !UNITY_5_3_OR_NEWER && UNITY_5_3
#define UNITY_5_3_OR_NEWER
#endif

using UnityEngine;
using UnityEditor;
using WhiteCatEditor;

namespace WhiteCat.Tween
{
	/// <summary>
	/// TweenFromTo Editor
	/// </summary>
	public partial class TweenFromTo<T>
	{
		const float _toggleRatio = 0.17f;
		const float _intervalRatio = 0.08f;
		const float _fromLabelWidth = 35;
		const float _toLabelWidth = 20;

		static Rect _rect;
		static float _labelWidth;
		static float _lineWidth;
		static float _fieldWidth;


		SerializedProperty _tweenerProperty;
		protected SerializedProperty _fromProperty;
		protected SerializedProperty _toProperty;


		protected override void Editor_OnEnable()
		{
			_tweenerProperty = editor.serializedObject.FindProperty("_tweener");
			_fromProperty = editor.serializedObject.FindProperty("_from");
			_toProperty = editor.serializedObject.FindProperty("_to");
		}


		protected override void Editor_OnDisable()
		{
			_tweenerProperty = null;
			_fromProperty = null;
			_toProperty = null;
		}


		protected sealed override void Editor_OnInspectorGUI()
		{
			editor.serializedObject.Update();

			EditorGUILayout.PropertyField(_tweenerProperty);
			EditorGUILayout.Space();
			DrawExtraFields();

			editor.serializedObject.ApplyModifiedProperties();
		}


		protected virtual void DrawExtraFields()
		{
			EditorGUILayout.PropertyField(_fromProperty, true);
			EditorGUILayout.PropertyField(_toProperty, true);
		}


		protected static void FloatChannelField(SerializedProperty toggle, string label, SerializedProperty from, SerializedProperty to)
		{
			_labelWidth = EditorGUIUtility.labelWidth;

			_rect = EditorGUILayout.GetControlRect();
			_lineWidth = _rect.width;
			_fieldWidth = (_lineWidth * (1f - _intervalRatio - _intervalRatio - _toggleRatio) - _fromLabelWidth - _toLabelWidth) * 0.5f;

			_rect.width = _lineWidth * _toggleRatio;
			toggle.boolValue = EditorGUI.ToggleLeft(_rect, label, toggle.boolValue);

			EditorGUIUtility.labelWidth = _fromLabelWidth;
			_rect.x = _rect.xMax + _lineWidth * _intervalRatio;
			_rect.width = _fieldWidth + _fromLabelWidth;
			from.floatValue = EditorGUI.FloatField(_rect, "From", from.floatValue);

			EditorGUIUtility.labelWidth = _toLabelWidth;
			_rect.x = _rect.xMax + _lineWidth * _intervalRatio;
			_rect.width = _fieldWidth + _toLabelWidth;
			to.floatValue = EditorGUI.FloatField(_rect, "To", to.floatValue);

			EditorGUIUtility.labelWidth = _labelWidth;
		}


		protected static void ClampedFloatChannelField(SerializedProperty toggle, string label, SerializedProperty from, SerializedProperty to, float min, float max)
		{
			_labelWidth = EditorGUIUtility.labelWidth;

			_rect = EditorGUILayout.GetControlRect();
			_lineWidth = _rect.width;
			_fieldWidth = (_lineWidth * (1f - _intervalRatio - _intervalRatio - _toggleRatio) - _fromLabelWidth - _toLabelWidth) * 0.5f;

			_rect.width = _lineWidth * _toggleRatio;
			toggle.boolValue = EditorGUI.ToggleLeft(_rect, label, toggle.boolValue);

			EditorGUIUtility.labelWidth = _fromLabelWidth;
			_rect.x = _rect.xMax + _lineWidth * _intervalRatio;
			_rect.width = _fieldWidth + _fromLabelWidth;
			from.floatValue = Mathf.Clamp(EditorGUI.FloatField(_rect, "From", from.floatValue), min, max);

			EditorGUIUtility.labelWidth = _toLabelWidth;
			_rect.x = _rect.xMax + _lineWidth * _intervalRatio;
			_rect.width = _fieldWidth + _toLabelWidth;
			to.floatValue = Mathf.Clamp(EditorGUI.FloatField(_rect, "To", to.floatValue), min, max);

			EditorGUIUtility.labelWidth = _labelWidth;
		}


		protected static void ColorRGBField(SerializedProperty toggle, SerializedProperty from, SerializedProperty to, bool hdr)
		{
			_labelWidth = EditorGUIUtility.labelWidth;

			_rect = EditorGUILayout.GetControlRect();
			_lineWidth = _rect.width;
			_fieldWidth = (_lineWidth * (1f - _intervalRatio - _intervalRatio - _toggleRatio) - _fromLabelWidth - _toLabelWidth) * 0.5f;

			_rect.width = _lineWidth * _toggleRatio;
			toggle.boolValue = EditorGUI.ToggleLeft(_rect, "RGB", toggle.boolValue);

			EditorGUIUtility.labelWidth = _fromLabelWidth;
			_rect.x = _rect.xMax + _lineWidth * _intervalRatio;
			_rect.width = _fieldWidth + _fromLabelWidth;

#if UNITY_5_3_OR_NEWER
			if (hdr)
			{
				from.colorValue = EditorGUI.ColorField(_rect, EditorKit.GlobalContent("From"), from.colorValue, true, true, true, EditorKit.colorPickerHDRConfig);
			}
			else
#endif
			{
				from.colorValue = EditorGUI.ColorField(_rect, "From", from.colorValue);
			}

			EditorGUIUtility.labelWidth = _toLabelWidth;
			_rect.x = _rect.xMax + _lineWidth * _intervalRatio;
			_rect.width = _fieldWidth + _toLabelWidth;

#if UNITY_5_3_OR_NEWER
			if (hdr)
			{
				to.colorValue = EditorGUI.ColorField(_rect, EditorKit.GlobalContent("To"), to.colorValue, true, true, true, EditorKit.colorPickerHDRConfig);
			}
			else
#endif
			{
				to.colorValue = EditorGUI.ColorField(_rect, "To", to.colorValue);
			}

			EditorGUIUtility.labelWidth = _labelWidth;
		}

	} // class TweenFromTo

} // namespace WhiteCat.Tween

#endif // UNITY_EDITOR