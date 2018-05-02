#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using WhiteCatEditor;
#endif

namespace WhiteCat
{
	/// <summary>
	/// SerializableClassWithEditor
	/// </summary>
	[System.Serializable]
	public class SerializableClassWithEditor
	{

#if UNITY_EDITOR

		static PropertyDrawerForSerializableClass _drawer;


		/// <summary>
		/// 在实现编辑器方法时用以访问当前 PropertyDrawer 对象
		/// </summary>
		protected static PropertyDrawerForSerializableClass drawer
		{
			get { return _drawer; }
		}


		/// <summary>
		/// Editor_GetPropertyHeight
		/// </summary>
		protected virtual float Editor_GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return EditorGUI.GetPropertyHeight(property, label, true);
		}


		/// <summary>
		/// Editor_OnGUI
		/// </summary>
		protected virtual void Editor_OnGUI(Rect rect, SerializedProperty property, GUIContent label)
		{
			EditorGUI.PropertyField(rect, property, label, true);
        }


		/// <summary>
		/// PropertyDrawerForSerializableClass
		/// </summary>
		[CustomPropertyDrawer(typeof(SerializableClassWithEditor), true)]
		protected class PropertyDrawerForSerializableClass : BasePropertyDrawer
		{
			public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
			{
				_drawer = this;
				float value = GetFieldValue<SerializableClassWithEditor>(property).Editor_GetPropertyHeight(property, label);
				_drawer = null;
				return value;
			}


			public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
			{
				_drawer = this;
				GetFieldValue<SerializableClassWithEditor>(property).Editor_OnGUI(rect, property, label);
				_drawer = null;
			}

		} // class PropertyDrawerForSerializableClass

#endif // UNITY_EDITOR

	} // class SerializableClassWithEditor

} // namespace WhiteCat