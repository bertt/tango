using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace WhiteCat.Paths
{
	/// <summary>
	/// 在路径上按指定速度移动
	/// </summary>
	[AddComponentMenu("White Cat/Path/Move Along Path/Move Along Path with Speed")]
	public class MoveAlongPathWithSpeed : MoveAlongPath
	{
		/// <summary>
		/// 移动速度
		/// </summary>
		public float speed = 1f;


		/// <summary>
		/// 更新模式
		/// </summary>
		public UpdateMode updateMode = UpdateMode.Update;


		/// <summary>
		/// 时间模式
		/// </summary>
		public TimeMode timeMode = TimeMode.Normal;


		void Update()
		{
			if (updateMode == UpdateMode.Update)
			{
				distance += speed * (timeMode == TimeMode.Normal ? Time.deltaTime : Time.unscaledDeltaTime);
			}
		}


		void LateUpdate()
		{
			if (updateMode == UpdateMode.LateUpdate)
			{
				distance += speed * (timeMode == TimeMode.Normal ? Time.deltaTime : Time.unscaledDeltaTime);
			}
		}


		void FixedUpdate()
		{
			if (updateMode == UpdateMode.FixedUpdate)
			{
				distance += speed * Time.fixedDeltaTime;
			}
		}


#if UNITY_EDITOR

		SerializedProperty _speedProperty;
		SerializedProperty _updateModeProperty;
		SerializedProperty _timeModeProperty;


		protected override void Editor_OnEnable()
		{
			_speedProperty = editor.serializedObject.FindProperty("speed");
			_updateModeProperty = editor.serializedObject.FindProperty("updateMode");
			_timeModeProperty = editor.serializedObject.FindProperty("timeMode");
		}


		protected override void Editor_OnDisable()
		{
			_speedProperty = null;
			_updateModeProperty = null;
			_timeModeProperty = null;
		}


		protected override void Editor_OnExtraInspectorGUI()
		{
			editor.serializedObject.Update();
			EditorGUILayout.PropertyField(_speedProperty);
			EditorGUILayout.PropertyField(_updateModeProperty);
			EditorGUI.BeginDisabledGroup(updateMode == UpdateMode.FixedUpdate);
			EditorGUILayout.PropertyField(_timeModeProperty);
			EditorGUI.EndDisabledGroup();
			editor.serializedObject.ApplyModifiedProperties();
		}

#endif

	} // class MoveAlongPathWithSpeed

} // namespace WhiteCat.Paths