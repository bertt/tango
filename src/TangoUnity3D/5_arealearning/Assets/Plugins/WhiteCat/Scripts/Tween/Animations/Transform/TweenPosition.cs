using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace WhiteCat.Tween
{
	/// <summary>
	/// 位置插值动画
	/// </summary>
	[AddComponentMenu("White Cat/Tween/Transform/Tween Position")]
	public class TweenPosition : TweenVector3
	{
		public Space space = Space.Self;


		public override Vector3 current
		{
			get
			{
				return space == Space.Self ? transform.localPosition : transform.position;
			}
			set
			{
				if (space == Space.Self) transform.localPosition = value;
				else transform.position = value;
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

	} // class TweenPosition

} // namespace WhiteCat.Tween