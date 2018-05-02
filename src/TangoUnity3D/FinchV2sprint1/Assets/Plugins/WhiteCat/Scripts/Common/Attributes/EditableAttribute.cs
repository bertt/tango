using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace WhiteCat
{
	/// <summary>
	/// 标记一个字段, 设置在编辑模式和播放模式下是否可以编辑
	/// </summary>
	[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
	public sealed class EditableAttribute : PropertyAttributeWithEditor
	{

#if UNITY_EDITOR

		bool _editMode;
		bool _playMode;


		public EditableAttribute(bool editMode, bool playMode)
		{
			_editMode = editMode;
			_playMode = playMode;
		}


		protected override void Editor_OnGUI(Rect rect, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginDisabledGroup(!(Application.isPlaying ? _playMode : _editMode));
			EditorGUI.PropertyField(rect, property, label, true);
			EditorGUI.EndDisabledGroup();
		}

#else

		public EditableAttribute(bool editMode, bool playMode)
		{
		}

#endif // UNITY_EDITOR

	} // class EditableAttribute

} // namespace WhiteCat