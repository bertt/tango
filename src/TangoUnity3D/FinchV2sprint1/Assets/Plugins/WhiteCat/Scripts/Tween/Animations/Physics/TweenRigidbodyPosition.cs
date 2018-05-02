using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace WhiteCat.Tween
{
	/// <summary>
	/// 刚体位置插值动画
	/// 应当使用 FixedUpdate 来更新此动画
	/// </summary>
	[AddComponentMenu("White Cat/Tween/Physics/Tween Rigidbody Position")]
	[RequireComponent(typeof(Rigidbody))]
	public class TweenRigidbodyPosition : TweenVector3
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


		public override Vector3 current
		{
			get
			{
				return space == Space.Self ? transform.localPosition : transform.position;
			}
			set
			{
#if UNITY_EDITOR
				if (!Application.isPlaying)
				{
					if (space == Space.Self) transform.localPosition = value;
					else transform.position = value;
				}
				else
#endif
				{
					targetRigidbody.MovePosition(
						(space == Space.Self && transform.parent) ?
						transform.parent.TransformPoint(value) : value);
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
			DrawFromToChannels();
		}

#endif // UNITY_EDITOR

	} // class TweenRigidbodyPosition

} // namespace WhiteCat.Tween