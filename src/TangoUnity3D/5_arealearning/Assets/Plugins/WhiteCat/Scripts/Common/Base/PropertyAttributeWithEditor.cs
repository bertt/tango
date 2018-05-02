using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using WhiteCatEditor;
#endif

namespace WhiteCat
{
	/// <summary>
	/// PropertyAttributeWithEditor
	/// </summary>
	public class PropertyAttributeWithEditor : PropertyAttribute
	{

#if UNITY_EDITOR

		static PropertyDrawerForAttribute _drawer;


		/// <summary>
		/// 在实现编辑器方法时用以访问当前 PropertyDrawer 对象
		/// </summary>
		protected static PropertyDrawerForAttribute drawer
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
		/// PropertyDrawerForAttribute
		/// </summary>
		[CustomPropertyDrawer(typeof(PropertyAttributeWithEditor), true)]
		protected class PropertyDrawerForAttribute : BasePropertyDrawer
		{
			public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
			{
				_drawer = this;
                float value = (attribute as PropertyAttributeWithEditor).Editor_GetPropertyHeight(property, label);
				_drawer = null;
				return value;
			}


			public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
			{
				_drawer = this;
				(attribute as PropertyAttributeWithEditor).Editor_OnGUI(rect, property, label);
				_drawer = null;
			}

		} // class PropertyDrawerForAttribute

#endif // UNITY_EDITOR

	} // class PropertyAttributeWithEditor

} // namespace WhiteCat