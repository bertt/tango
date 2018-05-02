using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace WhiteCat.Tween
{
	/// <summary>
	/// Transform 插值动画
	/// </summary>
	[AddComponentMenu("White Cat/Tween/Transform/Tween Transform")]
	public class TweenTransform : Tweener.TweenAnimation
	{
		public Transform from;
		public Transform to;


		Vector3 _originalPosition;
		Quaternion _originalRotation;
		Vector3 _originalScale;


		public override void OnTween(float factor)
		{
			if (from && to)
			{
				transform.position = (to.position - from.position) * factor + from.position;
				transform.rotation = Quaternion.SlerpUnclamped(from.rotation, to.rotation, factor);
				transform.localScale = (to.localScale - from.localScale) * factor + from.localScale;
			}
		}


		public override void OnRecord()
		{
			_originalPosition = transform.position;
			_originalRotation = transform.rotation;
			_originalScale = transform.localScale;
		}


		public override void OnRestore()
		{
			transform.position = _originalPosition;
			transform.rotation = _originalRotation;
			transform.localScale = _originalScale;
		}


		[ContextMenu("Set 'From' to current")]
		void SetFromToCurrent()
		{
			if (from)
			{
				from.position = transform.position;
				from.rotation = transform.rotation;
				from.localScale = transform.localScale;
			}
		}


		[ContextMenu("Set 'To' to current")]
		void SetToToCurrent()
		{
			if (to)
			{
				to.position = transform.position;
				to.rotation = transform.rotation;
				to.localScale = transform.localScale;
			}
		}


		[ContextMenu("Set current to 'From'")]
		void SetCurrentToFrom()
		{
			if (from)
			{
				transform.position = from.position;
				transform.rotation = from.rotation;
				transform.localScale = from.localScale;
			}
		}


		[ContextMenu("Set current to 'To'")]
		void SetCurrentToTo()
		{
			if (to)
			{
				transform.position = to.position;
				transform.rotation = to.rotation;
				transform.localScale = to.localScale;
			}
		}


		protected void Reset()
		{
			from = to = null;
		}

#if UNITY_EDITOR

		SerializedProperty _tweenerProperty;
		SerializedProperty _fromProperty;
		SerializedProperty _toProperty;


		protected override void Editor_OnEnable()
		{
			_tweenerProperty = editor.serializedObject.FindProperty("_tweener");
			_fromProperty = editor.serializedObject.FindProperty("from");
			_toProperty = editor.serializedObject.FindProperty("to");
		}


		protected override void Editor_OnDisable()
		{
			_tweenerProperty = null;
			_fromProperty = null;
			_toProperty = null;
		}


		protected override void Editor_OnInspectorGUI()
		{
			editor.serializedObject.Update();
			EditorGUILayout.PropertyField(_tweenerProperty);
			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(_fromProperty);
			EditorGUILayout.PropertyField(_toProperty);
			editor.serializedObject.ApplyModifiedProperties();
		}

#endif // UNITY_EDITOR

	} // class TweenTransform

} // namespace WhiteCat.Tween