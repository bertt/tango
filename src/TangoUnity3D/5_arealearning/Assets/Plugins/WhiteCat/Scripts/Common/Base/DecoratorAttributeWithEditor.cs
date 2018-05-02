using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace WhiteCat
{
	/// <summary>
	/// DecoratorAttributeWithEditor
	/// </summary>
	public class DecoratorAttributeWithEditor : PropertyAttribute
	{

#if UNITY_EDITOR

		static DecoratorDrawerForAttribute _drawer;


		/// <summary>
		/// 在实现编辑器方法时用以访问当前 DecoratorDrawer 对象
		/// </summary>
		protected static DecoratorDrawerForAttribute drawer
		{
			get { return _drawer; }
		}


		/// <summary>
		/// Editor_GetHeight
		/// </summary>
		protected virtual float Editor_GetHeight()
		{
			return 0f;
		}


		/// <summary>
		/// Editor_OnGUI
		/// </summary>
		protected virtual void Editor_OnGUI(Rect rect)
		{
        }


		/// <summary>
		/// DecoratorDrawerForAttribute
		/// </summary>
		[CustomPropertyDrawer(typeof(DecoratorAttributeWithEditor), true)]
		protected class DecoratorDrawerForAttribute : DecoratorDrawer
		{
			public override float GetHeight()
			{
				_drawer = this;
				float value = (attribute as DecoratorAttributeWithEditor).Editor_GetHeight();
				_drawer = null;
				return value;
			}


			public override void OnGUI(Rect rect)
			{
				_drawer = this;
				(attribute as DecoratorAttributeWithEditor).Editor_OnGUI(rect);
				_drawer = null;
			}

		} // class DecoratorDrawerForAttribute

#endif // UNITY_EDITOR

	} // class DecoratorAttributeWithEditor

} // namespace WhiteCat