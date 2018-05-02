using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace WhiteCat.Tween
{
	/// <summary>
	/// 欧拉角插值动画
	/// </summary>
	[AddComponentMenu("White Cat/Tween/Transform/Tween Euler Angles")]
	public class TweenEulerAngles : TweenVector3
	{
		public Space space = Space.Self;


		public override Vector3 current
		{
			get
			{
				return space == Space.Self ? transform.localEulerAngles : transform.eulerAngles;
			}
			set
			{
				if (space == Space.Self) transform.localEulerAngles = value;
				else transform.eulerAngles = value;
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
			DrawFromToChannels();
		}

#endif // UNITY_EDITOR

	} // class TweenEulerAngles

} // namespace WhiteCat.Tween