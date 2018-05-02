using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace WhiteCat.Tween
{
	/// <summary>
	/// 旋转四元数插值动画
	/// </summary>
	[AddComponentMenu("White Cat/Tween/Transform/Tween Rotation")]
	public class TweenRotation : TweenQuaternion
	{
		public Space space = Space.Self;


		public override Quaternion current
		{
			get
			{
				return space == Space.Self ? transform.localRotation : transform.rotation;
			}
			set
			{
				if (space == Space.Self) transform.localRotation = value;
				else transform.rotation = value;
			}
		}

#if UNITY_EDITOR

		SerializedProperty _spaceProperty;

		protected override void Editor_OnEnable()
		{
			base.Editor_OnEnable();

			_spaceProperty = editor.serializedObject.FindProperty("space");
		}


		protected override void Editor_OnDisable()
		{
			base.Editor_OnDisable();
			_spaceProperty = null;
		}


		protected override void DrawExtraFields()
		{
			EditorGUILayout.PropertyField(_spaceProperty);
			DrawFromToAngles();
		}

#endif // UNITY_EDITOR

	} // class TweenRotation

} // namespace WhiteCat.Tween