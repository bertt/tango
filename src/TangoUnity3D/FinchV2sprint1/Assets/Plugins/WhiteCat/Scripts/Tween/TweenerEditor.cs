#if UNITY_EDITOR

using System.Text;
using UnityEngine;
using UnityEditor;
using WhiteCatEditor;

namespace WhiteCat.Tween
{
	/// <summary>
	/// Tweener Editor
	/// </summary>
	public partial class Tweener
	{
		[SerializeField] Color _personalizedColor = EditorKit.randomColor;

		int _selectedPageIndex = 1;
		bool _showAnimations = false;
		bool _isPreviewInEditor = false;
		protected bool _isDraggingInEditor = false;
		bool _enabledRecord;
		double _lastTime;

		SerializedProperty _interpolatorProp;
		SerializedProperty _startDelayProp;
		SerializedProperty _durationProp;
		SerializedProperty _wrapModeProp;
		SerializedProperty _playModeProp;
		SerializedProperty _updateModeProp;
		SerializedProperty _timeModeProp;
		SerializedProperty _onForwardToEndingProp;
		SerializedProperty _onBackToBeginningProp;

		static string[] pageTitles = { "Interpolator", "Settings", "Events" };
		static Color progressBackground = new Color(0, 0, 0, 0.15f);
		static StringBuilder stringBuilder = new StringBuilder("Element ", 16);


		bool previewInEditor
		{
			set
			{
				if (_isPreviewInEditor != value)
				{
					_isPreviewInEditor = value;

					if (value)
					{
						EditorApplication.update += UpdateInEditor;
						EditorApplication.playmodeStateChanged += StopPreview;
						_lastTime = EditorApplication.timeSinceStartup;

						_enabledRecord = enabled;
						RecordAnimationStates();
						enabled = true;
					}
					else
					{
						EditorApplication.update -= UpdateInEditor;
						EditorApplication.playmodeStateChanged -= StopPreview;

						_normalizedTime = 0;
						_isForward = true;

						enabled = _enabledRecord;
						RestoreAnimationStates();
					}
				}
			}
		}


		bool draggingInEditor
		{
			set
			{
				if (_isDraggingInEditor != value)
				{
					_isDraggingInEditor = value;

					if (value)
					{
						if (!Application.isPlaying && !_isPreviewInEditor)
						{
							RecordAnimationStates();
						}
					}
					else
					{
						if (!Application.isPlaying && !_isPreviewInEditor)
						{
							_normalizedTime = 0;
							_isForward = true;
							RestoreAnimationStates();
						}
					}
                }
			}
		}


		void StopPreview()
		{
			previewInEditor = false;
        }


		void UpdateInEditor()
		{
			if (!_isDraggingInEditor)
			{
				UpdateTween((float)(EditorApplication.timeSinceStartup - _lastTime));
			}
			_lastTime = EditorApplication.timeSinceStartup;
		}


		protected override void Editor_OnEnable()
		{
			_interpolatorProp = editor.serializedObject.FindProperty("_interpolator");
			_startDelayProp = editor.serializedObject.FindProperty("_startDelay");
			_durationProp = editor.serializedObject.FindProperty("_duration");
			_wrapModeProp = editor.serializedObject.FindProperty("_wrapMode");
			_playModeProp = editor.serializedObject.FindProperty("_playMode");
			_updateModeProp = editor.serializedObject.FindProperty("_updateMode");
			_timeModeProp = editor.serializedObject.FindProperty("_timeMode");
			_onForwardToEndingProp = editor.serializedObject.FindProperty("_onForwardToEnding");
			_onBackToBeginningProp = editor.serializedObject.FindProperty("_onBackToBeginning");
		}


		protected override void Editor_OnDisable()
		{
			if (!Application.isPlaying) previewInEditor = false;

			_interpolatorProp = null;
			_startDelayProp = null;
			_durationProp = null;
			_wrapModeProp = null;
			_playModeProp = null;
			_updateModeProp = null;
			_timeModeProp = null;
			_onForwardToEndingProp = null;
			_onBackToBeginningProp = null;
		}


		protected override bool Editor_RequiresConstantRepaint()
		{
			if (Application.isPlaying) return isActiveAndEnabled;
			else return _isPreviewInEditor;
		}


		protected override void Editor_OnInspectorGUI()
		{
			Color foregroundColor = EditorKit.defaultContentColor;

			EditorGUILayout.Space();
			_selectedPageIndex = GUILayout.Toolbar(_selectedPageIndex, pageTitles, EditorStyles.miniButton);
			EditorGUILayout.Space();
			Rect rect = EditorGUILayout.GetControlRect(false, 1f);
			EditorGUI.DrawRect(rect, foregroundColor * 0.5f);
			EditorGUILayout.Space();

			editor.serializedObject.Update();

			switch (_selectedPageIndex)
			{
				case 0:		// Interpolator
					{
						EditorGUILayout.PropertyField(_interpolatorProp);
						break;
					}

				case 1:		// Animation
					{
						EditorGUILayout.PropertyField(_startDelayProp);
						EditorGUILayout.PropertyField(_durationProp);
						EditorGUILayout.Space();
						EditorGUILayout.PropertyField(_wrapModeProp);
						EditorGUILayout.PropertyField(_playModeProp);
						EditorGUILayout.Space();
						EditorGUILayout.PropertyField(_updateModeProp);
						EditorGUI.BeginDisabledGroup(_updateMode == UpdateMode.FixedUpdate);
						EditorGUILayout.PropertyField(_timeModeProp);
						EditorGUI.EndDisabledGroup();
						EditorGUILayout.Space();

						rect = EditorGUILayout.GetControlRect();
						EditorGUI.LabelField(rect, " ", _animations.Count.ToString());
						_showAnimations = EditorGUI.Foldout(rect, _showAnimations, "Animations", true);
						if (_showAnimations)
						{
							EditorGUI.indentLevel++;
							EditorGUI.BeginDisabledGroup(true);
							for (int i = 0; i < _animations.Count; i++)
							{
								stringBuilder.Length = 8;
                                EditorGUILayout.ObjectField(stringBuilder.Append(i).ToString(), _animations[i], typeof(TweenAnimation), false);
							}
							EditorGUI.EndDisabledGroup();
							EditorGUI.indentLevel--;
						}
						break;
					}

				case 2:		// Event
					{
						EditorGUILayout.PropertyField(_onForwardToEndingProp);
						EditorGUILayout.Space();
						EditorGUILayout.PropertyField(_onBackToBeginningProp);
						break;
					}
			}

			editor.serializedObject.ApplyModifiedProperties();

			EditorGUILayout.Space();
			rect = EditorGUILayout.GetControlRect(true, 1f);
			EditorGUI.DrawRect(rect, foregroundColor * 0.5f);

			EditorGUILayout.Space();
			rect = EditorGUILayout.GetControlRect(true);

			// 绘制预览按钮
			Rect previewRect = rect;
			previewRect.width = 54f;
			rect.xMin = rect.xMax - rect.height;

			if (Application.isPlaying)
			{
				EditorKit.RecordAndSetGUIBackgroundColor(enabled ? _personalizedColor : GUI.backgroundColor);
				enabled = GUI.Toggle(previewRect, enabled, "Play", EditorStyles.miniButton);
				EditorKit.RestoreGUIBackgroundColor();
			}
			else
			{
				EditorKit.RecordAndSetGUIBackgroundColor(_isPreviewInEditor ? _personalizedColor : GUI.backgroundColor);
				previewInEditor = GUI.Toggle(previewRect, _isPreviewInEditor, "Preview", EditorStyles.miniButton);
				EditorKit.RestoreGUIBackgroundColor();
			}

			// 进度条
			previewRect.x = previewRect.xMax + 8f;
			previewRect.xMax = rect.xMin - 8f;

			// 鼠标开始拖动
			if (Event.current.type == EventType.MouseDown)
			{
				if (previewRect.Contains(Event.current.mousePosition))
				{
					draggingInEditor = true;
				}
			}

			// 鼠标结束拖动
			if (Event.current.rawType == EventType.MouseUp)
			{
				if (_isDraggingInEditor)
				{
					draggingInEditor = false;
					editor.Repaint();
				}
			}

			// 绘制进度条
			EditorGUI.BeginChangeCheck();
			float time01 = EditorKit.ProgressBar(previewRect, _normalizedTime, progressBackground, _personalizedColor, foregroundColor, true);
			if (EditorGUI.EndChangeCheck() && _isDraggingInEditor)
			{
				normalizedTime = time01;
			}

			// 绘制个性化颜色按钮
			if(GUI.Button(rect, GUIContent.none))
			{
				Undo.RecordObject(this, "Change Color");
				_personalizedColor = EditorKit.randomColor;
				EditorUtility.SetDirty(this);
			}
				
			// 绘制颜色块
			EditorKit.RecordAndSetGUIColor(foregroundColor);
			GUI.DrawTexture(rect, EditorGUIUtility.whiteTexture);
			EditorKit.RestoreGUIColor();

			EditorKit.RecordAndSetGUIColor(_personalizedColor);
			previewRect.Set(rect.x + 1f, rect.y + 1f, rect.width - 2f, rect.height - 2f);
            GUI.DrawTexture(previewRect, EditorGUIUtility.whiteTexture);
			EditorKit.RestoreGUIColor();

			EditorGUILayout.Space();
		}

	} // class Tweener

} // namespace WhiteCat.Tween

#endif // UNITY_EDITOR