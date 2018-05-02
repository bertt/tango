using UnityEngine;
using WhiteCat;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace WhiteCat.Tween
{
	/// <summary>
	/// 时间缩放插值动画
	/// </summary>
	[AddComponentMenu("White Cat/Tween/Global/Tween Time Scale")]
	public class TweenTimeScale : TweenFloat
	{
		public bool keepFixedUpdateFrequency = true;

		[SerializeField][Clamp(0.1f, 10000f)]
		float _fixedUpdateFrequency = 50f;


		public override float from
		{
			get { return _from; }
			set { _from = Mathf.Clamp(value, 0f, 100f); }
		}


		public override float to
		{
			get { return _to; }
			set { _to = Mathf.Clamp(value, 0f, 100f); }
		}


		public float fixedUpdateFrequency
		{
			get { return _fixedUpdateFrequency; }
			set { _fixedUpdateFrequency = Mathf.Clamp(value, 0.1f, 10000f); }
		}


		public override float current
		{
			get { return Time.timeScale; }
			set
			{
				if (keepFixedUpdateFrequency)
				{
					Time.fixedDeltaTime = Mathf.Clamp(value / _fixedUpdateFrequency, 0.0001f, 10f);
				}

				Time.timeScale = value;
			}
		}

#if UNITY_EDITOR

		SerializedProperty _keepFixedUpdateFrequencyProperty;
		SerializedProperty _fixedUpdateFrequencyProperty;


		protected override void Editor_OnEnable()
		{
			base.Editor_OnEnable();

			_keepFixedUpdateFrequencyProperty = editor.serializedObject.FindProperty("keepFixedUpdateFrequency");
			_fixedUpdateFrequencyProperty = editor.serializedObject.FindProperty("_fixedUpdateFrequency");
		}


		protected override void Editor_OnDisable()
		{
			base.Editor_OnDisable();

			_keepFixedUpdateFrequencyProperty = null;
			_fixedUpdateFrequencyProperty = null;
		}


		protected override void DrawExtraFields()
		{
			if (tweener && (tweener.updateMode == UpdateMode.FixedUpdate || tweener.timeMode == TimeMode.Normal))
			{
				EditorGUILayout.HelpBox("Use non-fixed update-mode with unscaled time-mode for Tweener to avoid unexpected result.", MessageType.Warning, true);
				EditorGUILayout.Space();
			}

			EditorGUILayout.PropertyField(_keepFixedUpdateFrequencyProperty);
			if (keepFixedUpdateFrequency)
			{
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField(_fixedUpdateFrequencyProperty);
				EditorGUI.indentLevel--;
			}
			EditorGUILayout.Space();

			DrawClampedFromToValues(0f, 100f);
		}

#endif // UNITY_EDITOR

	} // class TweenTimeScale

} // namespace WhiteCat.Tween