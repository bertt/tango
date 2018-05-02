using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace WhiteCat.Tween
{
	[Serializable]
	public class MaterialTextureProperty : MaterialProperty
	{
#if UNITY_EDITOR
		protected override ShaderUtil.ShaderPropertyType propertyType
		{
			get { return ShaderUtil.ShaderPropertyType.TexEnv; }
		}
#endif
	}


	/// <summary>
	/// 材质 Texture Offset 插值动画
	/// </summary>
	[AddComponentMenu("White Cat/Tween/Material/Tween Material Texture Offset")]
	public class TweenMaterialTextureOffset : TweenVector2
	{
		[SerializeField]
		MaterialTextureProperty _property = new MaterialTextureProperty();


		public MaterialTextureProperty property
		{
			get { return _property; }
		}


		public override Vector2 current
		{
			get
			{
				var material = _property.material;
				return material ? material.GetTextureOffset(_property.propertyName) : Vector2.zero;
            }
			set
			{
				var material = _property.material;
				if (material)
				{
					material.SetTextureOffset(_property.propertyName, value);
					_property.ValidateDynamicGI();
				}
			}
		}

#if UNITY_EDITOR

		SerializedProperty _serializedProperty;


		protected override void Editor_OnEnable()
		{
			base.Editor_OnEnable();
			_serializedProperty = editor.serializedObject.FindProperty("_property");
		}


		protected override void Editor_OnDisable()
		{
			base.Editor_OnDisable();
			_serializedProperty = null;
		}


		protected override void DrawExtraFields()
		{
			EditorGUILayout.PropertyField(_serializedProperty);
			EditorGUILayout.Space();
			DrawFromToChannels();
		}

#endif // UNITY_EDITOR

	} // class TweenMaterialTextureOffset

} // namespace WhiteCat.Tween