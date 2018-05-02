using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace WhiteCat.Tween
{
	[Serializable]
	public class MaterialRangeProperty : MaterialProperty
	{
#if UNITY_EDITOR
		protected override ShaderUtil.ShaderPropertyType propertyType
		{
			get { return ShaderUtil.ShaderPropertyType.Range; }
		}
#endif
	}


	/// <summary>
	/// 材质 Range 类型属性插值动画
	/// </summary>
	[AddComponentMenu("White Cat/Tween/Material/Tween Material Range")]
	public class TweenMaterialRange : TweenFloat
	{
		[SerializeField]
		MaterialRangeProperty _property = new MaterialRangeProperty();


		public MaterialRangeProperty property
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

			int index = _property.editor_propertyIndex;
            if (index >= 0)
			{
				DrawClampedFromToValuesWithSlider(
					ShaderUtil.GetRangeLimits(_property.editor_shader, index, 1),
					ShaderUtil.GetRangeLimits(_property.editor_shader, index, 2));
			}
			else base.DrawExtraFields();
		}

#endif // UNITY_EDITOR

	} // class TweenMaterialRange

} // namespace WhiteCat.Tween