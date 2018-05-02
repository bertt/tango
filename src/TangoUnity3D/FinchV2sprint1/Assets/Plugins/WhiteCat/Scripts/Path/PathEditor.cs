#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using WhiteCatEditor;

namespace WhiteCat.Paths
{
	/// <summary>
	/// Path Editor
	/// </summary>
	public partial class Path
	{
		// Scene 参数

		const float _splineWidth = 2f;

		protected static Color _normalColor = new Color(1, 0.3f, 0.6f, 1f);
		protected static Color _highlightColor = new Color(1, 0.8f, 0f, 1f);

		protected const float _splineCapSize = 0.1f;
		protected const float _controlPointCapSize = 0.05f;

		protected static Color _capNormalColor = new Color(0.2f, 0.7f, 1f, 1f);
		protected static Color _capSelectedColor = new Color(1f, 0.5f, 0f, 1f);

		protected static Color _haloColor = new Color(1f, 0.5f, 0f, 0.25f);
		protected static Color _controlLineColor = new Color(0.75f, 0.75f, 0.75f, 0.75f);

		// Toolbar 参数

		const float _toolBarOffsetY = 17f;
		const float _toolBarHeight = 39f;

		const float _toolBarPaddingLeft = 0f;
		const float _toolBarPaddingRight = 10f;
		const float _toolBarPaddingTop = 0f;
		const float _toolBarPaddingBottom = 14f;

		protected const float _toolBarHorizontalInterval = 12f;
		protected const float _toolBarBigButtonWidth = 37f;
		protected const float _toolBarBigButtonHeight = 27f;
		protected const float _toolBarButtonHeight = 21f;
		protected const float _toolBarButtonWidth = 27f;
		protected const float _toolBarLineHeight = 16f;

		protected const float _tensionLabelWidth = 50f;
		protected const float _tensionWidth = 100f;
		protected const float _positionWidth = 177f;

		static GUIContent[] _toolBarContent;
		protected static GUIContent[] toolBarContent
		{
			get
			{
				if (_toolBarContent == null)
				{
					_toolBarContent = new GUIContent[4];
					for (int i = 0; i < 4; i++)
					{
						_toolBarContent[i] = new GUIContent();
					}

					_toolBarContent[0].image = EditorAssets.selectLineTexture;
					_toolBarContent[0].tooltip = "Select Curve Tool";

					_toolBarContent[1].image = EditorAssets.selectNodeTexture;
					_toolBarContent[1].tooltip = "Select Node Tool";

					_toolBarContent[2].image = EditorAssets.moveInPlaneTexture;
					_toolBarContent[2].tooltip = "Pan Tool";

					_toolBarContent[3].image = EditorAssets.moveInSpaceTexture;
					_toolBarContent[3].tooltip = "Move Tool";
				}
				return _toolBarContent;
			}
		}

		// Inspector 参数

		const float _editButtonHeight = 23f;
		const float _editButtonWidth = 33f;
		const float _horizontalInterval = 5f;

		static GUIContent _worldScaleContent
			= new GUIContent("World Scale", "Use \"World Scale\" to scale the path instead of scales of transform.");

		static GUIStyle _middleLeftLabel;
		static GUIStyle middleLeftLabel
		{
			get
			{
				if (_middleLeftLabel == null)
				{
					_middleLeftLabel = new GUIStyle(EditorStyles.label);
					_middleLeftLabel.alignment = TextAnchor.MiddleLeft;
				}
				return _middleLeftLabel;
			}
		}

		// 常值

		protected static Vector3 _zeroVector3 = Vector3.zero;
		protected static Quaternion _identityQuaternion = Quaternion.identity;

		// 全局变量

		static Matrix4x4 _pathMatrix;
		static Rect _rect;

		// Editor 参数

		static Object _currentEditing = null;
		bool _alwaysVisible = false;


		// 返回高亮 spline 的索引
		protected abstract int highlightSplineIndex { get; }

		// 在场景里绘制编辑控件
		protected abstract void DrawSceneControls();

		// 返回工具条总宽度
		protected abstract float toolBarWidth { get; }

		// 绘制工具条内容
		protected abstract void DrawToolBar(Rect rect);


		static void SetCurrentEditing(Object target)
		{
			if (_currentEditing != target)
			{
				_currentEditing = target;
				Tools.hidden = target;
				SceneView.RepaintAll();
			}
		}


		void DoDrawGizmos()
		{
			_pathMatrix.SetTRS(transform.position, transform.rotation, new Vector3(_worldScale, _worldScale, _worldScale));
			EditorKit.RecordAndSetHandlesMatrix(ref _pathMatrix);

			// 绘制路径

			int highlight = _currentEditing == this ? highlightSplineIndex : -1;

			for (int i = 0; i < _localSegments.Count; i++)
			{
				_localSegments[i].Draw(highlight == i ? _highlightColor : _normalColor, _splineWidth);
			}

			// 绘制箭头

			EditorKit.RecordAndSetHandlesColor(_highlightColor);

			Vector3 point = _localSegments[_localSegments.Count - 1].GetPoint(1f);
			float scale = HandleUtility.GetHandleSize(point) / Mathf.Abs(_worldScale);
			Vector3 vector = new Vector3(0.08f, 0f, - 0.24f) * scale;
			var rotation = Quaternion.LookRotation(_localSegments[_localSegments.Count - 1].GetTangent(1f), Vector3.up);
			Handles.DrawLine(point, point + rotation * vector);
			vector.x = -vector.x;
			Handles.DrawLine(point, point + rotation * vector);

			EditorKit.RestoreHandlesColor();

			EditorKit.RestoreHandlesMatrix();
		}


		[DrawGizmo(GizmoType.Selected | GizmoType.NonSelected)]
		static void DrawGizmos(Path target, GizmoType type)
		{
			if ((type & GizmoType.Selected) != 0 || target._alwaysVisible)
			{
				target.DoDrawGizmos();
			}
		}


		protected override void Editor_OnDisable()
		{
			if (_currentEditing == this) SetCurrentEditing(null);
		}


		protected sealed override void Editor_OnInspectorGUI()
		{
			// Edit Button

			_rect = EditorGUILayout.GetControlRect(true, _editButtonHeight);
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
			EditorGUI.LabelField(_rect, "Edit Path", middleLeftLabel);

			// Circular

			editor.DrawToggleLayout(circular, value => circular = value, "Circular");

			// World Scale

			EditorGUI.BeginChangeCheck();
			float scale = EditorGUILayout.FloatField(_worldScaleContent, worldScale);
			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(this, editor.undoString);
				worldScale = scale;
				EditorUtility.SetDirty(this);
			}

			// Length Error

			editor.DrawFloatFieldLayout(lengthError, value => lengthError = value, "Length Error");

			// Length

			if (isSamplesValid)
			{
				EditorGUILayout.FloatField("Length", length);
			}
			else
			{
				_rect = EditorGUILayout.GetControlRect();
				EditorGUI.LabelField(_rect, "Length");
				_rect.xMin += EditorGUIUtility.labelWidth;

				// Calculate Button

				if (GUI.Button(_rect, "Calculate", EditorStyles.miniButton))
				{
					Undo.RecordObject(this, editor.undoString);
					ValidateSamples();
					EditorUtility.SetDirty(this);
				}
			}

			// Always Visible Button
			_alwaysVisible = EditorKit.IndentedToggle("Always Visible", _alwaysVisible);
		}


		protected sealed override void Editor_OnSceneGUI()
		{
			if (_currentEditing == this)
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

	} // class Path

} // namespace WhiteCat.Paths

#endif // UNITY_EDITOR