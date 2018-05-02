#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using WhiteCatEditor;

namespace WhiteCat.Paths
{
	public partial class Path
	{
		/// <summary>
		/// Keyframe List Editor
		/// </summary>
		public partial class KeyframeList
		{
			// 编辑器个性化颜色
			public Color personalizedColor = EditorKit.randomColor;


			// 绘制额外的 Inspector
			protected virtual void DrawExtraInspector() { }

			// 在场景里绘制编辑控件
			protected abstract void DrawSceneControls();

			// 返回工具条总宽度
			protected abstract float toolBarWidth { get; }

			// 绘制工具条内容
			protected abstract void DrawToolBar(Rect rect);


			protected sealed override void Editor_OnDisable()
			{
				if (_currentEditing == this) SetCurrentEditing(null);
			}


			protected sealed override void Editor_OnInspectorGUI()
			{
				// Check Path

				if (!path)
				{
					EditorGUILayout.HelpBox("Require Path Component!", MessageType.Error, true);
					if (_currentEditing == this) SetCurrentEditing(null);
					return;
				}

				// personalized Color

				_rect = EditorGUILayout.GetControlRect(true, _editButtonHeight);
				var rect2 = new Rect(_rect.x + 3f, _rect.y + 3f, _rect.height - 6f, _rect.height - 6f);

				if (GUI.Button(rect2, GUIContent.none))
				{
					Undo.RecordObject(this, "Change Color");
					personalizedColor = EditorKit.randomColor;
					EditorUtility.SetDirty(this);
				}

				EditorKit.RecordAndSetGUIColor(EditorKit.defaultContentColor);
				GUI.DrawTexture(rect2, EditorGUIUtility.whiteTexture);
				EditorKit.RestoreGUIColor();

				EditorKit.RecordAndSetGUIColor(personalizedColor);
				rect2.Set(rect2.x + 1f, rect2.y + 1f, rect2.width - 2f, rect2.height - 2f);
				GUI.DrawTexture(rect2, EditorGUIUtility.whiteTexture);
				EditorKit.RestoreGUIColor();

				// Edit Button

				_rect.x += EditorGUIUtility.labelWidth;
				_rect.width = _editButtonWidth;

				EditorGUI.BeginChangeCheck();
				EditorKit.RecordAndSetGUIContentColor(EditorKit.defaultContentColor);
				bool edit = GUI.Toggle(_rect, _currentEditing == this, EditorAssets.editTexture, EditorKit.buttonStyle);
				EditorKit.RestoreGUIContentColor();
				if (EditorGUI.EndChangeCheck())
				{
					SetCurrentEditing(edit ? this : null);
				}

				// Edit Label

				_rect.x = _rect.xMax + _horizontalInterval;
				_rect.width = 128f;
				EditorGUI.LabelField(_rect, "Edit Keys", middleLeftLabel);

				// DrawExtraInspector

				DrawExtraInspector();

				// Sort Button

				if (!isSorted)
				{
					if (EditorKit.IndentedButton("Sort"))
					{
						Undo.RecordObject(this, editor.undoString);
						Sort();
						EditorUtility.SetDirty(this);
					}
				}
			}


			protected sealed override void Editor_OnSceneGUI()
			{
				if (_currentEditing == this && path)
				{
					// 屏蔽鼠标点选物体
					HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

					// 绘制场景控件
					DrawSceneControls();

					// 设置工具栏位置和大小
					float toolBarWidth = this.toolBarWidth;

					_rect.width = toolBarWidth + _toolBarPaddingLeft + _toolBarPaddingRight;
					_rect.height = _toolBarHeight + _toolBarPaddingTop + _toolBarPaddingBottom;
					_rect.x = -_toolBarPaddingLeft;
					_rect.y = _toolBarOffsetY - _toolBarPaddingTop;

					Rect localRect = new Rect(_toolBarPaddingLeft, _toolBarPaddingTop, toolBarWidth, _toolBarHeight);

					// 绘制工具栏
					Handles.BeginGUI();
					int toolBarID = GUIUtility.GetControlID(FocusType.Passive);
					GUI.Window(toolBarID, _rect, id => DrawToolBar(localRect), GUIContent.none, EditorAssets.toolBarGUIStyle);
					GUI.BringWindowToFront(toolBarID);
					Handles.EndGUI();
				}
			}

		} // class KeyframeList


		/// <summary>
		/// Keyframe List Editor
		/// </summary>
		public partial class KeyframeList<KeyValue, Keyframe, Component>
		{
			// 工具条参数
			const float _distanceWidth = 100f;
			const float _distanceLabelWidth = 50f;

			static Vector3 zeroVector3 = Vector3.zero;
			static Quaternion identityQuaternion = Quaternion.identity;
			static GUIContent globalGUIContent = new GUIContent();

			static Vector3 _point;
			static Vector3 _selectedPoint;
			static Rect _lineRect;

			int _selectedItem = 0;
			bool _showKeyDetail;


			// 在场景中绘制 KeyValue 的细节
			protected abstract void DrawKeyValueInScene(Vector3 position, KeyValue value, float handleSize);


			// 在场景中编辑 KeyValue
			protected abstract KeyValue DrawKeyValueHandle(Vector3 position, KeyValue value, float handleSize);


			// 值编辑器的宽度
			protected abstract float keyValueWidth { get; }


			// 绘制 key 的值
			protected abstract KeyValue DrawKeyValue(KeyValue value, Rect rect);


			// 在场景里绘制编辑控件
			protected sealed override void DrawSceneControls()
			{
				Path path = base.path;
				int count = _keyframes.Count;
				float pathLength = path.length;
				float position;
				float handleSize;

				_selectedItem = Mathf.Clamp(_selectedItem, 0, count - 1);

				for (int i = 0; i < count; i++)
				{
					position = _keyframes[i].position;
					if (position > pathLength) continue;

					EditorKit.BeginHotControlChangeCheck();

					_point = path.GetPoint(path.GetLocationByLength(position));

					if (_selectedItem == i)
					{
						Handles.color = _capSelectedColor;
						_selectedPoint = _point;
					}
					else Handles.color = personalizedColor;

					handleSize = HandleUtility.GetHandleSize(_point);
					Handles.FreeMoveHandle(_point, identityQuaternion, handleSize * _controlPointCapSize, zeroVector3, Handles.DotCap);

					if (EditorKit.EndHotControlChangeCheck() == HotControlEvent.MouseDown)
					{
						_selectedItem = i;
					}

					if (_showKeyDetail || _selectedItem == i)
					{
						DrawKeyValueInScene(_point, _keyframes[i].value, handleSize);
					}

					if (_selectedItem == i)
					{
						editor.SmartDraw(
							() => DrawKeyValueHandle(_point, _keyframes[i].value, handleSize),
							value => SetValue(i, value));
					}
				}

				// 绘制光晕
				Vector2 guiPoint = HandleUtility.WorldToGUIPoint(_selectedPoint);
				_lineRect.Set(guiPoint.x - 32f, guiPoint.y - 34f, 64f, 64f);

				Handles.BeginGUI();
				EditorKit.RecordAndSetGUIColor(_haloColor);
				GUI.DrawTexture(_lineRect, EditorAssets.roundGradientTexture);
				EditorKit.RestoreGUIColor();
				Handles.EndGUI();

				// 居中元素
				if (Event.current.type == EventType.KeyDown)
				{
					if (Event.current.character == 'f' || Event.current.character == 'F')
					{
						Event.current.Use();
						SceneView.lastActiveSceneView.LookAt(_selectedPoint);
					}
				}
			}


			// 返回工具条总宽度
			protected sealed override float toolBarWidth
			{
				get { return 7 * _toolBarHorizontalInterval + _distanceWidth + 5 * _toolBarButtonWidth + keyValueWidth + _toolBarBigButtonWidth; }
			}


			// 绘制工具条内容
			protected sealed override void DrawToolBar(Rect rect)
			{
				_selectedItem = Mathf.Clamp(_selectedItem, 0, count - 1);

				EditorKit.RecordAndSetGUIContentColor(EditorKit.defaultContentColor);

				// showKeyDetail

				globalGUIContent.image = EditorAssets.detailInfoTexture;
				globalGUIContent.tooltip = "Show Details";

				rect.Set(rect.x + _toolBarHorizontalInterval, rect.y + (rect.height - _toolBarBigButtonHeight) * 0.5f, _toolBarBigButtonWidth, _toolBarBigButtonHeight);
				_showKeyDetail = GUI.Toggle(rect, _showKeyDetail, globalGUIContent, EditorKit.buttonStyle);

				// Position

				EditorGUI.BeginChangeCheck();

				EditorKit.RecordAndSetLabelWidth(_distanceLabelWidth);
				_lineRect.Set(rect.xMax + _toolBarHorizontalInterval, rect.y + (rect.height - _toolBarLineHeight) * 0.5f, _distanceWidth, _toolBarLineHeight);
				float position = EditorGUI.FloatField(_lineRect, "Position", _keyframes[_selectedItem].position);
				EditorKit.RestoreLabelWidth();

				if (EditorGUI.EndChangeCheck())
				{
					Undo.RecordObject(this, "Set Position");
					SetPosition(_selectedItem, Mathf.Min(position, path.length));
					EditorUtility.SetDirty(this);
				}

				// Value

				EditorGUI.BeginChangeCheck();

				_lineRect.x = _lineRect.xMax + _toolBarHorizontalInterval;
				_lineRect.width = keyValueWidth;
				KeyValue value = DrawKeyValue(GetValue(_selectedItem), _lineRect);

				if (EditorGUI.EndChangeCheck())
				{
					Undo.RecordObject(this, "Set Value");
					SetValue(_selectedItem, value);
					EditorUtility.SetDirty(this);
				}

				// Insert Back

				rect.Set(_lineRect.xMax + _toolBarHorizontalInterval, rect.y + (rect.height - _toolBarButtonHeight) * 0.5f, _toolBarButtonWidth, _toolBarButtonHeight);
				globalGUIContent.image = EditorAssets.insertNodeBackTexture;
				globalGUIContent.tooltip = "Insert Back";

				if (GUI.Button(rect, globalGUIContent, EditorKit.buttonLeftStyle))
				{
					Undo.RecordObject(this, "Insert Key");
					InsertBack();
					EditorUtility.SetDirty(this);

					return;
				}

				// Insert Forward

				rect.x = rect.xMax;
				globalGUIContent.image = EditorAssets.insertNodeForwardTexture;
				globalGUIContent.tooltip = "Insert Forward";

				if (GUI.Button(rect, globalGUIContent, EditorKit.buttonRightStyle))
				{
					Undo.RecordObject(this, "Insert Key");
					InsertForward();
					EditorUtility.SetDirty(this);

					return;
				}

				// Remove

				rect.x = rect.xMax + _toolBarHorizontalInterval;
				globalGUIContent.image = EditorAssets.removeNodeTexture;
				globalGUIContent.tooltip = "Remove Key";

				EditorGUI.BeginDisabledGroup(_keyframes.Count == 1);

				if (GUI.Button(rect, globalGUIContent, EditorKit.buttonStyle))
				{
					Undo.RecordObject(this, "Remove Key");
					RemoveAt(_selectedItem);
					EditorUtility.SetDirty(this);

					return;
				}

				EditorGUI.EndDisabledGroup();

				// 前一个

				rect.x = rect.xMax + _toolBarHorizontalInterval;
				globalGUIContent.image = EditorAssets.prevTexture;
				globalGUIContent.tooltip = "Previous key";

				if (GUI.Button(rect, globalGUIContent, EditorKit.buttonLeftStyle))
				{
					Sort();
					_selectedItem = (_selectedItem == 0) ? (_keyframes.Count - 1) : (_selectedItem - 1);
				}

				// 后一个

				rect.x = rect.xMax;
				globalGUIContent.image = EditorAssets.nextTexture;
				globalGUIContent.tooltip = "Next key";

				if (GUI.Button(rect, globalGUIContent, EditorKit.buttonRightStyle))
				{
					Sort();
					_selectedItem = (_selectedItem == _keyframes.Count - 1) ? 0 : (_selectedItem + 1);
				}

				EditorKit.RestoreGUIContentColor();
			}


			void InsertBack()
			{
				float position;
				KeyValue value;

				float nextDistance = _keyframes[_selectedItem].position;
				Sort();
				int nextIndex = _keyframes.FindIndex(item => item.position == nextDistance);

				if (path.circular)
				{
					if (_keyframes.Count == 1)
					{
						position = (_keyframes[0].position + path.length * 0.5f) % path.length;
						value = _keyframes[0].value;
					}
					else
					{
						if (nextIndex == 0)
						{
							position = (_keyframes[0].position + path.length + _keyframes[_keyframes.Count - 1].position) * 0.5f % path.length;
							value = Lerp(_keyframes[0].value, _keyframes[_keyframes.Count - 1].value, 0.5f);
						}
						else
						{
							position = (nextDistance + _keyframes[nextIndex - 1].position) * 0.5f;
							value = Lerp(_keyframes[nextIndex].value, _keyframes[nextIndex - 1].value, 0.5f);
						}
					}
				}
				else
				{
					if (nextIndex == 0)
					{
						position = 0f;
						value = _keyframes[0].value;
					}
					else
					{
						position = (nextDistance + _keyframes[nextIndex - 1].position) * 0.5f;
						value = Lerp(_keyframes[nextIndex].value, _keyframes[nextIndex - 1].value, 0.5f);
					}
				}

				Add(position, value);
				_selectedItem = _keyframes.Count - 1;
			}


			void InsertForward()
			{
				float position;
				KeyValue value;

				float prevDistance = _keyframes[_selectedItem].position;
				Sort();
				int prevIndex = _keyframes.FindLastIndex(item => item.position == prevDistance);

				if (path.circular)
				{
					if (_keyframes.Count == 1)
					{
						position = (_keyframes[0].position + path.length * 0.5f) % path.length;
						value = _keyframes[0].value;
					}
					else
					{
						if (prevIndex == _keyframes.Count - 1)
						{
							position = (_keyframes[0].position + path.length + _keyframes[_keyframes.Count - 1].position) * 0.5f % path.length;
							value = Lerp(_keyframes[0].value, _keyframes[_keyframes.Count - 1].value, 0.5f);
						}
						else
						{
							position = (prevDistance + _keyframes[prevIndex + 1].position) * 0.5f;
							value = Lerp(_keyframes[prevIndex].value, _keyframes[prevIndex + 1].value, 0.5f);
						}
					}
				}
				else
				{
					if (prevIndex == _keyframes.Count - 1)
					{
						position = path.length;
						value = _keyframes[prevIndex].value;
					}
					else
					{
						position = (prevDistance + _keyframes[prevIndex + 1].position) * 0.5f;
						value = Lerp(_keyframes[prevIndex].value, _keyframes[prevIndex + 1].value, 0.5f);
					}
				}

				Add(position, value);
				_selectedItem = _keyframes.Count - 1;
			}

		} // class KeyframeList<KeyValue, Keyframe>

	} // class Path

} // namespace WhiteCat.Paths

#endif