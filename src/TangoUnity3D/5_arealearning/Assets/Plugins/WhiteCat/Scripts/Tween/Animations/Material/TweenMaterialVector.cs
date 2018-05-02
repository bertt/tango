using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace WhiteCat.Tween
{
	[Serializable]
	public class MaterialVectorProperty : MaterialProperty
	{
#if UNITY_EDITOR
		protected override ShaderUtil.ShaderPropertyType propertyType
		{
			get { return ShaderUtil.ShaderPropertyType.Vector; }
		}
#endif
	}


	/// <summary>
	/// 材质 Vector 类型属性插值动画
	/// </summary>
	[AddComponentMenu("White Cat/Tween/Material/Tween Material Vector")]
	public class TweenMaterialVector : TweenVector4
	{
		[SerializeField]
		MaterialVectorProperty _property = new MaterialVectorProperty();


		public MaterialVectorProperty property
		{
			get { return _property; }
		}


		public override Vector4 current
		{
			get
			{
				var material = _property.material;
				return material ? material.GetVector(_property.propertyID) : Vector4.zero;
            }
			set
			{
				var material = _property.material;
				if (material)
				{
					material.SetVector(_property.propertyID, value);
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

	} // class TweenMaterialVector

} // namespace WhiteCat.Tween