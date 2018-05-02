using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace WhiteCat.Tween
{
	[Serializable]
	public class MaterialColorProperty : MaterialProperty
	{
#if UNITY_EDITOR
		protected override ShaderUtil.ShaderPropertyType propertyType
		{
			get { return ShaderUtil.ShaderPropertyType.Color; }
		}
#endif
	}


	/// <summary>
	/// 材质 Color 类型属性插值动画
	/// </summary>
	[AddComponentMenu("White Cat/Tween/Material/Tween Material Color")]
	public class TweenMaterialColor : TweenColor
	{
		[SerializeField]
		MaterialColorProperty _property = new MaterialColorProperty();


		public MaterialColorProperty property
		{
			get { return _property; }
		}


		public override Color current
		{
			get
			{
				var material = _property.material;
                return material ? material.GetColor(_property.propertyID) : Color.white;
            }
			set
			{
				var material = _property.material;
				if (material)
				{
					material.SetColor(_property.propertyID, value);
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

	} // class TweenMaterialColor

} // namespace WhiteCat.Tween