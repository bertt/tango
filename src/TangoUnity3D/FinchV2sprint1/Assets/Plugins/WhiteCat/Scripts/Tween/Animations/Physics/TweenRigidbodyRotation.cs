using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace WhiteCat.Tween
{
	/// <summary>
	/// 刚体旋转插值动画
	/// 应当使用 FixedUpdate 来更新此动画
	/// </summary>
	[AddComponentMenu("White Cat/Tween/Physics/Tween Rigidbody Rotation")]
	[RequireComponent(typeof(Rigidbody))]
	public class TweenRigidbodyRotation : TweenQuaternion
	{
		public Space space = Space.World;


		Rigidbody _rigidbody;
		public Rigidbody targetRigidbody
		{
			get
			{
				if (!_rigidbody)
				{
					_rigidbody = GetComponent<Rigidbody>();
				}
				return _rigidbody;
			}
		}


		public override Quaternion current
		{
			get
			{
				return space == Space.Self ? transform.localRotation : transform.rotation;
			}
			set
			{
#if UNITY_EDITOR
				if (!Application.isPlaying)
				{
					if (space == Space.Self) transform.localRotation = value;
					else transform.rotation = value;
				}
				else
#endif
				{
					targetRigidbody.MoveRotation(
						(space == Space.Self && transform.parent) ?
						transform.parent.rotation * value : value);
				}
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

	} // class TweenRigidbodyRotation

} // namespace WhiteCat.Tween