#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using WhiteCatEditor;
using System;

namespace WhiteCat.Paths
{
	/// <summary>
	/// Cardinal Path Editor
	/// </summary>
	public partial class CardinalPath
	{
		// 当前选择的对象类型
		enum SelectionType
		{
			None,
			Spline,
			Node
		}

		// 常量
		static string undoNode = "Node";

		// 全局变量
		static int _count;
		static float _handleSize;
		static Location _location;
		static Rect _lineRect;
		static Vector3 _node;

		// 当前选择的工具类型 [0 ~ 3]
		int _selectedTool = 2;

		// 当前选择的对象类型
		SelectionType _selectionType = SelectionType.None;

		// 当前选择的元素 (当 selectionType != None 时有效)
		int _selectedItem = -1;


		// 验证选择数据是否正确
		void ValidateSelectedItem()
		{
			switch (_selectionType)
			{
				case SelectionType.None:
					{
						_selectedItem = -1;
						return;
					}
				case SelectionType.Spline:
					{
						_selectedItem = Mathf.Clamp(_selectedItem, 0, segmentCount - 1);
						return;
					}
				default:
					{
						_selectedItem = Mathf.Clamp(_selectedItem, 0, nodeCount - 1);
						return;
					}
			}
		}


		// 返回高亮 spline 的索引
		protected override int highlightSplineIndex
		{
			get
			{
				ValidateSelectedItem();

				if (_selectionType == SelectionType.Spline)
				{
					return _selectedItem;
				}
				else
				{
					return -1;
				}
			}
		}


		// 在场景里绘制编辑控件
		protected override void DrawSceneControls()
		{
			ValidateSelectedItem();

			// 绘制控件

			switch (_selectedTool)
			{
				case 0: DrawSplineControls(); break;
				case 1: DrawNodeControls(false); break;
				case 2: DrawNodeControls(true); break;
				case 3: DrawNodeControls(false); DrawMoveHandle(); break;
			}

			if (_selectionType != SelectionType.None)
			{
				// 获取选择元素的世界位置

				Vector3 worldPoint;

				switch (_selectionType)
				{
					case SelectionType.Spline:
						{
							_location.Set(_selectedItem, 0.5f);
							worldPoint = GetPoint(_location);
							break;
						}
					case SelectionType.Node:
						{
							worldPoint = GetNodePosition(_selectedItem);
							break;
						}
					default:
						{
							worldPoint = Vector3.zero;
							break;
						}
				}

				// 绘制光晕

				Vector2 guiPoint = HandleUtility.WorldToGUIPoint(worldPoint);
				_lineRect = new Rect(guiPoint.x - 32f, guiPoint.y - 34f, 64f, 64f);

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
						SceneView.lastActiveSceneView.LookAt(worldPoint);
					}
				}
			}
		}


		// 绘制 Spline 控件
		void DrawSplineControls()
		{
			_location.time = 0.5f;
			_count = segmentCount;

			for (_location.index = 0; _location.index < _count; _location.index++)
			{
				EditorKit.BeginHotControlChangeCheck();

				if (_selectedItem == _location.index) Handles.color = _capSelectedColor;
				else Handles.color = _capNormalColor;

				_handleSize = HandleUtility.GetHandleSize(_node = GetPoint(_location)) * _splineCapSize;
				Handles.FreeMoveHandle(_node, _identityQuaternion, _handleSize, _zeroVector3, Handles.CircleCap);

				if (EditorKit.EndHotControlChangeCheck() == HotControlEvent.MouseDown)
				{
					_selectionType = SelectionType.Spline;
					_selectedItem = _location.index;
				}
			}
		}


		// 绘制 Node 控件
		void DrawNodeControls(bool edit)
		{
			_count = nodeCount;

			for (int i = 0; i < _count; i++)
			{
				// 获取 Node 位置
				_node = GetNodePosition(i);

				// 绘制 Node

				EditorGUI.BeginChangeCheck();
				EditorKit.BeginHotControlChangeCheck();

				if (_selectionType == SelectionType.Node && _selectedItem == i) Handles.color = _capSelectedColor;
				else Handles.color = _capNormalColor;

				_handleSize = HandleUtility.GetHandleSize(_node) * _controlPointCapSize;
				_node = Handles.FreeMoveHandle(_node, _identityQuaternion, _handleSize, _zeroVector3, Handles.DotCap);

				// 更新选择
				if (EditorKit.EndHotControlChangeCheck() == HotControlEvent.MouseDown)
				{
					_selectionType = SelectionType.Node;
					_selectedItem = i;
				}

				// 修改 Node
				if (EditorGUI.EndChangeCheck() && edit)
				{
					Undo.RecordObject(this, undoNode);
					SetNodePosition(i, _node);
					EditorUtility.SetDirty(this);
				}
			}
		}


		// 绘制移动控件
		void DrawMoveHandle()
		{
			switch (_selectionType)
			{
				case SelectionType.Node:
					{
						EditorGUI.BeginChangeCheck();
						_node = Handles.PositionHandle(GetNodePosition(_selectedItem), transform.rotation);
						if (EditorGUI.EndChangeCheck())
						{
							Undo.RecordObject(this, undoNode);
							SetNodePosition(_selectedItem, _node);
							EditorUtility.SetDirty(this);
						}
						break;
					}

				default: break;
			}
		}


		// 返回顶部工具条总宽度
		protected override float toolBarWidth
		{
			get
			{
				ValidateSelectedItem();

				switch (_selectionType)
				{
					case SelectionType.Spline:
						{
							return _toolBarBigButtonWidth * 4f + _tensionWidth + _toolBarButtonWidth * 3f + _toolBarHorizontalInterval * 5f;
						}

					case SelectionType.Node:
						{
							return _toolBarBigButtonWidth * 4f + _positionWidth + _toolBarButtonWidth * 5f + _toolBarHorizontalInterval * 6f;
						}

					default:
						{
							return _toolBarBigButtonWidth * 4f + _toolBarHorizontalInterval * 2f;
						}
				}
			}
		}


		// 绘制顶部工具条内容
		protected override void DrawToolBar(Rect rect)
		{
			ValidateSelectedItem();
			EditorKit.RecordAndSetGUIContentColor(EditorKit.defaultContentColor);

			// 主工具栏
			rect.Set(rect.x + _toolBarHorizontalInterval, rect.y + (rect.height - _toolBarBigButtonHeight) * 0.5f, _toolBarBigButtonWidth * 4f, _toolBarBigButtonHeight);
			_selectedTool = GUI.Toolbar(rect, _selectedTool, toolBarContent);

			// 重置选择
			switch (_selectedTool)
			{
				case 0:
					{
						if (_selectionType != SelectionType.Spline)
						{
							_selectionType = SelectionType.None;
							_selectedItem = -1;
						}
						break;
					}
				case 1:
				case 2:
				case 3:
					{
						if (_selectionType == SelectionType.Spline)
						{
							_selectionType = SelectionType.None;
							_selectedItem = -1;
						}
						break;
					}
			}

			// 绘制其他控件
			switch (_selectionType)
			{
				case SelectionType.Spline:
					{
						DrawSplineUI(rect);
						break;
					}
				case SelectionType.Node:
					{
						DrawNodeUI(rect);
						break;
					}
			}

			EditorKit.RestoreGUIContentColor();
		}


		// 绘制选中路径段时的控件
		void DrawSplineUI(Rect rect)
		{
			_lineRect.Set(rect.xMax + _toolBarHorizontalInterval, rect.y + (rect.height - _toolBarLineHeight) * 0.5f, _tensionWidth, _toolBarLineHeight);

			// Tension

			EditorGUI.BeginChangeCheck();
			EditorKit.RecordAndSetLabelWidth(_tensionLabelWidth);
			float tension = EditorGUI.FloatField(_lineRect, "Tension", GetTension(_selectedItem));
			EditorKit.RestoreLabelWidth();
			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(this, "Tension");
				SetTension(_selectedItem, tension);
				EditorUtility.SetDirty(this);
			}

			// Insert Node

			rect.Set(_lineRect.xMax + _toolBarHorizontalInterval, rect.y + (rect.height - _toolBarButtonHeight) * 0.5f, _toolBarButtonWidth, _toolBarButtonHeight);

			if (GUI.Button(rect, EditorKit.GlobalContent(null, EditorAssets.insertNodeTexture, "Insert node"), EditorKit.buttonStyle))
			{
				Undo.RecordObject(this, "Insert Node");

				_selectedTool = 2;
				_selectionType = SelectionType.Node;
				InsertNode(_selectedItem += 1);

				EditorUtility.SetDirty(this);

				return;
			}

			// 前一个

			rect.x = rect.xMax + _toolBarHorizontalInterval;

			if (GUI.Button(rect, EditorKit.GlobalContent(null, EditorAssets.prevTexture, "Previous segment"), EditorKit.buttonLeftStyle))
			{
				_selectedItem = (_selectedItem == 0) ? (segmentCount - 1) : (_selectedItem - 1);
			}

			// 后一个

			rect.x = rect.xMax;

			if (GUI.Button(rect, EditorKit.GlobalContent(null, EditorAssets.nextTexture, "Next segment"), EditorKit.buttonRightStyle))
			{
				_selectedItem = (_selectedItem == segmentCount - 1) ? 0 : (_selectedItem + 1);
			}
		}


		// 绘制选中中控制点时的控件
		void DrawNodeUI(Rect rect)
		{
			// Position

			_lineRect.Set(rect.xMax + _toolBarHorizontalInterval, rect.y + (rect.height - _toolBarLineHeight) * 0.5f, _positionWidth, _toolBarLineHeight);

			EditorGUI.BeginChangeCheck();
			_node = EditorGUI.Vector3Field(_lineRect, GUIContent.none, GetNodePosition(_selectedItem, Space.Self));
			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(this, undoNode);
				SetNodePosition(_selectedItem, _node, Space.Self);
				EditorUtility.SetDirty(this);
			}

			// Insert Back

			rect.Set(_lineRect.xMax + _toolBarHorizontalInterval, rect.y + (rect.height - _toolBarButtonHeight) * 0.5f, _toolBarButtonWidth, _toolBarButtonHeight);

			if (GUI.Button(rect, EditorKit.GlobalContent(null, EditorAssets.insertNodeBackTexture, "Insert Back"), EditorKit.buttonLeftStyle))
			{
				Undo.RecordObject(this, "Insert Node");
				InsertNode(_selectedItem);
				EditorUtility.SetDirty(this);

				return;
			}

			// Insert Forward

			rect.x = rect.xMax;

			if (GUI.Button(rect, EditorKit.GlobalContent(null, EditorAssets.insertNodeForwardTexture, "Insert Forward"), EditorKit.buttonRightStyle))
			{
				Undo.RecordObject(this, "Insert Node");
				InsertNode(_selectedItem += 1);
				EditorUtility.SetDirty(this);

				return;
			}

			// Remove Node

			rect.x = rect.xMax + _toolBarHorizontalInterval;

			EditorGUI.BeginDisabledGroup(nodeCount <= 2);

			if (GUI.Button(rect, EditorKit.GlobalContent(null, EditorAssets.removeNodeTexture, "Remove node"), EditorKit.buttonStyle))
			{
				Undo.RecordObject(this, "Remove Node");
				RemoveNode(_selectedItem);
				EditorUtility.SetDirty(this);

				return;
			}

			EditorGUI.EndDisabledGroup();

			// 前一个

			rect.x = rect.xMax + _toolBarHorizontalInterval;

			if (GUI.Button(rect, EditorKit.GlobalContent(null, EditorAssets.prevTexture, "Previous node"), EditorKit.buttonLeftStyle))
			{
				_selectedItem = (_selectedItem == 0) ? (nodeCount - 1) : (_selectedItem - 1);
			}

			// 后一个

			rect.x = rect.xMax;

			if (GUI.Button(rect, EditorKit.GlobalContent(null, EditorAssets.nextTexture, "Next node"), EditorKit.buttonRightStyle))
			{
				_selectedItem = (_selectedItem == nodeCount - 1) ? 0 : (_selectedItem + 1);
			}
		}

	} // class CardinalPath

} // namespace WhiteCat.Paths

#endif // UNITY_EDITOR