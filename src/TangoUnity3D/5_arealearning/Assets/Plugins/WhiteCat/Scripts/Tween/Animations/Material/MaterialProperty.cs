using System;
using UnityEngine;

#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using WhiteCatEditor;
#endif

namespace WhiteCat.Tween
{
	/// <summary>
	/// 材质属性
	/// </summary>
	[Serializable]
	public abstract class MaterialProperty : SerializableClassWithEditor
	{
		// 材质类型
		[SerializeField] MaterialType _materialType = MaterialType.Specified;
		[SerializeField] bool _updateDynamicGI;

		// 指定材质
		[SerializeField] Material _specifiedMaterial;

		// 渲染器材质
		[SerializeField] Renderer _renderer;
		[SerializeField] int _materialIndex;

		// 材质
		Material _material;

		// 属性名称
		[SerializeField] string _propertyName;

		// 属性 ID
		int _propertyID = -1;


		/// <summary>
		/// 材质类型
		/// </summary>
		public MaterialType materialType
		{
			get { return _materialType; }
			set
			{
				_materialType = value;
				_material = null;
			}
		}


		/// <summary>
		/// 指定材质
		/// </summary>
		public Material specifiedMaterial
		{
			get { return _specifiedMaterial; }
			set
			{
				_specifiedMaterial = value;
				_material = null;
			}
		}


		/// <summary>
		/// 渲染器
		/// </summary>
		public Renderer renderer
		{
			get { return _renderer; }
			set
			{
				_renderer = value;
				_material = null;
			}
		}


		/// <summary>
		/// 检查动态 GI 是否需要更新
		/// </summary>
		public void ValidateDynamicGI()
		{
			if (_updateDynamicGI && _materialType != MaterialType.Specified)
			{
				//RendererExtensions.UpdateGIMaterials(_renderer);
			}
		}


		/// <summary>
		/// 渲染器中的材质索引
		/// </summary>
		public int materialIndex
		{
			get { return _materialIndex; }
			set
			{
				_materialIndex = value;
				_material = null;

				if (_renderer != null)
				{
					_materialIndex = Mathf.Clamp(value, 0, _renderer.sharedMaterials.Length - 1);
				}
			}
		}


		/// <summary>
		/// 如果 Renderer 的材质被修改需要手动调用此方法
		/// </summary>
		public void UpdateMaterial()
		{
			_material = null;

			if (_materialType == MaterialType.Specified)
			{
				_material = _specifiedMaterial;
			}
			else if (_renderer)
			{
				Material[] materials =
					(Application.isPlaying && (_materialType == MaterialType.RendererUnique)) ?
						_renderer.materials : _renderer.sharedMaterials;

				if (_materialIndex >= 0 && _materialIndex < materials.Length)
				{
					_material = materials[_materialIndex];
				}
			}
		}


		/// <summary>
		/// 材质引用. 如果使用渲染器独立材质, 编辑模式下返回的是共享材质, 运行时返回的是独立材质
		/// 注意, 应当在恰当的时候销毁独立材质
		/// </summary>
		public Material material
		{
			get
			{
				if (!_material) UpdateMaterial();
				return _material;
			}
		}


		/// <summary>
		/// 属性名 (不同于编辑器中显示的名称)
		/// </summary>
		public string propertyName
		{
			get { return _propertyName; }
			set
			{
				_propertyName = value;
				_propertyID = -1;

#if UNITY_EDITOR
				_propertyIndexInMenu = -2;
#endif
			}
		}


		/// <summary>
		/// 属性 ID
		/// </summary>
		public int propertyID
		{
			get
			{
				if (_propertyID == -1)
				{
					_propertyID = Shader.PropertyToID(_propertyName);
				}
				return _propertyID;
			}
		}

#if UNITY_EDITOR

		const float _lineInterval = 2f;

		// 缓存的 Shader 数据
		Shader _shader;
		List<int> _propertyIndexes = new List<int>(8);
		int _propertyCount;
        string[] _propertyNames;
		string[] _propertyDescriptions;

		// 当前属性在菜单中的索引. -2 表示未初始化, -1 表示不存在
		int _propertyIndexInMenu = -2;


		public Shader editor_shader { get { return _shader; } }
		public int editor_propertyIndex
		{
			get
			{
				return (!_shader || _propertyIndexInMenu < 0) ?
					-1 : _propertyIndexes[_propertyIndexInMenu];
			}
		}


		// 属性类型
		protected abstract ShaderUtil.ShaderPropertyType propertyType { get; }


		// 更新 Shader 数据
		void UpdateShader()
		{
			if (material)
			{
				if (_material.shader != _shader)
				{
					_shader = _material.shader;
					_propertyIndexInMenu = -2;

					if (_shader)
					{
						_propertyIndexes.Clear();
						_propertyCount = ShaderUtil.GetPropertyCount(_shader);

						for (int i = 0; i < _propertyCount; i++)
						{
							if (!ShaderUtil.IsShaderPropertyHidden(_shader, i) && ShaderUtil.GetPropertyType(_shader, i) == propertyType)
							{
								_propertyIndexes.Add(i);
							}
						}

						_propertyCount = _propertyIndexes.Count;
						if (_propertyCount > 0)
						{
							_propertyNames = new string[_propertyCount];
							_propertyDescriptions = new string[_propertyCount];

							for (int i = 0; i < _propertyCount; i++)
							{
								_propertyNames[i] = ShaderUtil.GetPropertyName(_shader, _propertyIndexes[i]);
								_propertyDescriptions[i] = string.Format("{0} ({1})",
									ShaderUtil.GetPropertyDescription(_shader, _propertyIndexes[i]),
									_propertyNames[i]);
							}
						}
					}
				}
			}
			else _shader = null;
		}


		// 获取编辑器高度
		protected override float Editor_GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			float lines = _materialType == MaterialType.Specified ? 4f : 6f;
			return lines * (EditorGUIUtility.singleLineHeight + _lineInterval) - _lineInterval;
		}


		// 绘制编辑器 UI
		protected override void Editor_OnGUI(Rect rect, SerializedProperty property, GUIContent label)
		{
			UnityEngine.Object target = property.serializedObject.targetObject;
			rect.height = EditorGUIUtility.singleLineHeight;

			{
				// 材质类型

				EditorGUI.BeginChangeCheck();
				var newMaterialType = EditorGUI.EnumPopup(rect, "Material Type", _materialType);
				if (EditorGUI.EndChangeCheck())
				{
					Undo.RecordObject(target, "Material Type");
					materialType = (MaterialType)newMaterialType;
					EditorUtility.SetDirty(target);
				}
			}

			rect.y = rect.yMax + _lineInterval;

			if (_materialType == MaterialType.Specified)
			{
				// 指定材质

				EditorGUI.BeginChangeCheck();
				var newSpecifiedMaterial = EditorGUI.ObjectField(rect, "Specified Material", _specifiedMaterial, typeof(Material), false);
				if (EditorGUI.EndChangeCheck())
				{
					Undo.RecordObject(target, "Specified Material");
					specifiedMaterial = newSpecifiedMaterial as Material;
					EditorUtility.SetDirty(target);
				}
			}
			else
			{
				// 动态 GI

				EditorGUI.BeginChangeCheck();
				bool newUpdateDynamicGI = EditorGUI.Toggle(rect, "Update Dynamic GI", _updateDynamicGI);
				if (EditorGUI.EndChangeCheck())
				{
					Undo.RecordObject(target, "Update Dynamic GI");
					_updateDynamicGI = newUpdateDynamicGI;
					EditorUtility.SetDirty(target);
				}

				rect.y = rect.yMax + _lineInterval;

				// 渲染器

				EditorGUI.BeginChangeCheck();
				var newRenderer = EditorGUI.ObjectField(rect, "Renderer", _renderer, typeof(Renderer), !EditorUtility.IsPersistent(target));
				if (EditorGUI.EndChangeCheck())
				{
					Undo.RecordObject(target, "Renderer");
					renderer = newRenderer as Renderer;
					EditorUtility.SetDirty(target);
				}

				rect.y = rect.yMax + _lineInterval;

				// 材质索引

				EditorGUI.BeginChangeCheck();
				int newMaterialIndex = EditorGUI.IntField(rect, "Material Index", _materialIndex);
				if (EditorGUI.EndChangeCheck())
				{
					Undo.RecordObject(target, "Material Index");
					materialIndex = newMaterialIndex;
					EditorUtility.SetDirty(target);
				}
			}

			rect.y = rect.yMax + _lineInterval;

			{
				// Shader

				UpdateShader();
				EditorGUI.BeginDisabledGroup(true);
				EditorGUI.ObjectField(rect, "Shader", _shader, typeof(Shader), false);
				EditorGUI.EndDisabledGroup();

				Rect refreshRect = rect;
				refreshRect.xMin = refreshRect.xMax - refreshRect.height;

				// 刷新按钮

				EditorGUI.DrawRect(refreshRect, EditorKit.defaultBackgroundColor);
				EditorKit.RecordAndSetGUIContentColor(EditorKit.defaultContentColor);
                if (GUI.Button(refreshRect, EditorAssets.refreshTexture, GUIStyle.none))
				{
					UpdateMaterial();
					_shader = null;
					UpdateShader();
				}
				EditorKit.RestoreGUIContentColor();
            }

			rect.y = rect.yMax + _lineInterval;

			if (_shader && _propertyCount > 0)
			{
				// 属性列表

				if (_propertyIndexInMenu == -2)
				{
					_propertyIndexInMenu = Array.FindIndex(_propertyNames, value => value == _propertyName);
                }

				EditorGUI.BeginChangeCheck();
				_propertyIndexInMenu = EditorGUI.Popup(rect, "Property", _propertyIndexInMenu, _propertyDescriptions);
				if (EditorGUI.EndChangeCheck())
				{
					Undo.RecordObject(target, "Property");
					propertyName = _propertyNames[_propertyIndexInMenu];
					EditorUtility.SetDirty(target);
				}
			}
			else EditorGUI.LabelField(rect, "- No available property.");

			// 同步 Undo Redo 时的数据
			if (Event.current.type == EventType.ValidateCommand && Event.current.commandName == "UndoRedoPerformed")
			{
				_material = null;
				_propertyID = -1;
				_propertyIndexInMenu = -2;
			}
		}

#endif // UNITY_EDITOR

	} // class MaterialProperty

} // namespace WhiteCat.Tween