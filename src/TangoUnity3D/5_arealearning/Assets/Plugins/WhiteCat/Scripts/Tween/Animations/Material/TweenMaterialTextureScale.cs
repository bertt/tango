using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace WhiteCat.Tween
{
	/// <summary>
	/// 材质 Texture Scale 插值动画
	/// </summary>
	[AddComponentMenu("White Cat/Tween/Material/Tween Material Texture Scale")]
	public class TweenMaterialTextureScale : TweenVector2
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
				return material ? material.GetTextureScale(_property.propertyName) : Vector2.one;
            }
			set
			{
				var material = _property.material;
				if (material)
				{
					material.SetTextureScale(_property.propertyName, value);
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

	} // class TweenMaterialTextureScale

} // namespace WhiteCat.Tween