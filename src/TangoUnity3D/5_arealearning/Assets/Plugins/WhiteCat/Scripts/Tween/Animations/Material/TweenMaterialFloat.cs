using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace WhiteCat.Tween
{
	[Serializable]
	public class MaterialFloatProperty : MaterialProperty
	{
#if UNITY_EDITOR
		protected override ShaderUtil.ShaderPropertyType propertyType
		{
			get { return ShaderUtil.ShaderPropertyType.Float; }
		}
#endif
	}


	/// <summary>
	/// 材质 float 类型属性插值动画
	/// </summary>
	[AddComponentMenu("White Cat/Tween/Material/Tween Material Float")]
	public class TweenMaterialFloat : TweenFloat
	{
		[SerializeField]
		MaterialFloatProperty _property = new MaterialFloatProperty();


		public MaterialFloatProperty property
		{
			get { return _property; }
		}


		public override float current
		{
			get
			{
				var material = _property.material;
				return material ? material.GetFloat(_property.propertyID) : 0f;
            }
			set
			{
				var material = _property.material;
				if (material)
				{
					material.SetFloat(_property.propertyID, value);
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
			base.DrawExtraFields();
		}

#endif // UNITY_EDITOR

	} // class TweenMaterialFloat

} // namespace WhiteCat.Tween