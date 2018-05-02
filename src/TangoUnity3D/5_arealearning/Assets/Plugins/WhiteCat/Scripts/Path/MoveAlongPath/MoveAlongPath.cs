using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using WhiteCatEditor;
#endif

namespace WhiteCat.Paths
{
	/// <summary>
	/// 在路径上移动
	/// </summary>
	[AddComponentMenu("White Cat/Path/Move Along Path/Move Along Path")]
	[DisallowMultipleComponent]
	public class MoveAlongPath : ScriptableComponentWithEditor
	{
		[SerializeField] Path _path = null;
		[SerializeField] float _distance = 0;
		[SerializeField] Location _location = new Location();

		[SerializeField] List<KeyframeListTargetComponentPair> _pairs = new List<KeyframeListTargetComponentPair>(4);


		/// <summary>
		/// 引用的路径
		/// </summary>
		public Path path
		{
			get { return _path; }
			set
			{
				if (_path != value)
				{
					if (value && value.transform.IsChildOf(transform))
					{
						Debug.LogError("The Path can neither be on itself, nor its children.");
						return;
					}

					_path = value;
					_pairs.Clear();
					Sync();
				}
			}
		}


		/// <summary>
		/// 从路径起点开始的距离
		/// </summary>
		public float distance
		{
			get { return _distance; }
			set
			{
				_distance = value;
				Sync();
            }
		}


		/// <summary>
		/// 路径位置参数
		/// </summary>
		public Location location
		{
			get { return _location; }
		}


		/// <summary>
		/// 立即执行同步, 当路径发生变化后可手动执行
		/// </summary>
		public void Sync()
		{
			if (_path)
			{
				if (!_path.circular) _distance = Mathf.Clamp(_distance, 0f, _path.length);

				_location = _path.GetLocationByLength(_distance, _location.index);
				transform.position = _path.GetPoint(_location);

				// 更新移动数据块
				KeyframeListTargetComponentPair pair;
				for (int i=0; i<_pairs.Count; i++)
				{
					pair = _pairs[i];
					if (pair.keyframeList && pair.targetComponent)
					{
						pair.keyframeList.UpdateMovingObject(pair, this);
					}
					else
					{
						_pairs.RemoveAt(i--);
					}
				}
			}
		}


		/// <summary>
		/// 添加移动对象. 物体移动中会计算路径关键帧插值并应用到目标组件上
		/// </summary>
		public bool AddMovingObject(Path.KeyframeList keyframeList, Component target)
		{
			if (_path && keyframeList && target
				&& keyframeList.path == _path
				&& keyframeList.targetComponentType.IsInstanceOfType(target)
				&& !_pairs.Exists(item => item.keyframeList == keyframeList))
			{
				var movingObject = new KeyframeListTargetComponentPair();
				movingObject.keyframeList = keyframeList;
				movingObject.targetComponent = target;
				_pairs.Add(movingObject);

				return true;
			}
			else return false;
		}


		/// <summary>
		/// 移除移动对象
		/// </summary>
		public void RemoveMovingObject(Path.KeyframeList keyframeList)
		{
			_pairs.RemoveAll(item => item.keyframeList == keyframeList);
		}


		/// <summary>
		/// 移除移动对象
		/// </summary>
		public void RemoveMovingObject(Component target)
		{
			_pairs.RemoveAll(item => item.targetComponent == target);
		}


#if UNITY_EDITOR

		static GUIContent _buttonContent = new GUIContent("Sync", "If you had modified the Path, click this button to sync position of Transform.");

		static Color _tableBackgroundColor = new Color(0.5f, 0.5f, 0.5f, 0.25f);
		static List<Path.KeyframeList> _keyframeLists = new List<Path.KeyframeList>(4);


		protected virtual void Editor_OnExtraInspectorGUI() { }


		protected sealed override void Editor_OnInspectorGUI()
		{
			// path
			editor.DrawObjectFieldLayout(_path, value => path = value, "Path");

			// distance
			EditorGUI.BeginChangeCheck();
			var distance = EditorGUILayout.FloatField("Distance", _distance);
			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(transform, editor.undoString);
				Undo.RecordObject(this, editor.undoString);
				this.distance = distance;
				EditorUtility.SetDirty(transform);
				EditorUtility.SetDirty(this);
			}

			Editor_OnExtraInspectorGUI();

			// sync button
			Rect rect = EditorGUILayout.GetControlRect(true);
			rect.xMin += EditorGUIUtility.labelWidth;
			if (GUI.Button(rect, _buttonContent, EditorStyles.miniButton))
			{
				Undo.RecordObject(transform, editor.undoString);
				Undo.RecordObject(this, editor.undoString);
				Sync();
				EditorUtility.SetDirty(transform);
				EditorUtility.SetDirty(this);
			}

			// keyframe list
			if (_path)
			{
				_keyframeLists.Clear();
				_path.GetComponents(_keyframeLists);

				if (_keyframeLists.Count > 0)
				{
					// 计算表格大小

					EditorGUILayout.Space();
					float lineHeight = EditorGUIUtility.singleLineHeight;
					rect = EditorGUILayout.GetControlRect(true, (lineHeight + 1f) * (_keyframeLists.Count + 1) + 4f);
					Rect left = new Rect(rect.x, rect.y, rect.width * 0.5f, lineHeight);
					Rect right = new Rect(left.xMax, left.y, left.width, lineHeight);

					// 绘制背景和线条

					EditorGUI.DrawRect(rect, _tableBackgroundColor);
					float yMax = rect.yMax;
					rect.height = 1f;
					Color lineColor = EditorKit.defaultContentColor * 0.5f;
					EditorGUI.DrawRect(rect, lineColor);
					rect.y += lineHeight + 1f;
					EditorGUI.DrawRect(rect, lineColor);
					rect.y = yMax;
					EditorGUI.DrawRect(rect, lineColor);

					// 绘制标题

					EditorGUI.LabelField(left, "Keyframe List", EditorKit.centeredBoldLabelStyle);
					EditorGUI.LabelField(right, "Target Component", EditorKit.centeredBoldLabelStyle);

					left.y += 3f;
					right.y += 3f;

					// 计算左列宽度

					rect = left;
					rect.width = lineHeight;
					left.xMin += lineHeight + 2f;

					// 绘制行元素

					Path.KeyframeList keyframeList;
					KeyframeListTargetComponentPair pair;

					for (int i = 0; i < _keyframeLists.Count; i++)
					{
						rect.y += lineHeight + 1f;
						left.y += lineHeight + 1f;
						right.y += lineHeight + 1f;

						keyframeList = _keyframeLists[i];

						// 绘制个性色

						EditorKit.RecordAndSetGUIColor(EditorKit.defaultContentColor);
						GUI.DrawTexture(rect, EditorAssets.bigDiamondTexture);
						EditorKit.RestoreGUIColor();

						EditorKit.RecordAndSetGUIColor(keyframeList.personalizedColor);
						GUI.DrawTexture(rect, EditorAssets.smallDiamondTexture);
						EditorKit.RestoreGUIColor();

						// 绘制关键帧列表名

						EditorGUI.LabelField(left, keyframeList.targetPropertyName);

						// 绘制目标组件引用

						pair = _pairs.Find(item => item.keyframeList == keyframeList);

						if (pair == null)
						{
							EditorGUI.BeginChangeCheck();
							var target = EditorGUI.ObjectField(right, null, keyframeList.targetComponentType, !EditorUtility.IsPersistent(this));
							if (EditorGUI.EndChangeCheck())
							{
								if (target)
								{
									Undo.RecordObject(this, editor.undoString);
									AddMovingObject(keyframeList, target as Component);
									EditorUtility.SetDirty(this);
									editor.Repaint();
									break;
								}
							}
						}
						else
						{
							EditorGUI.BeginChangeCheck();
							var target = EditorGUI.ObjectField(right, pair.targetComponent, keyframeList.targetComponentType, !EditorUtility.IsPersistent(this));
							if (EditorGUI.EndChangeCheck())
							{
								Undo.RecordObject(this, editor.undoString);

								if (target) pair.targetComponent = target as Component;
								else _pairs.Remove(pair);

								EditorUtility.SetDirty(this);
								editor.Repaint();
								break;
							}
						}
					}

					EditorGUILayout.Space();
				}
			}
		}

#endif

		} // class MoveAlongPath

} // namespace WhiteCat.Paths