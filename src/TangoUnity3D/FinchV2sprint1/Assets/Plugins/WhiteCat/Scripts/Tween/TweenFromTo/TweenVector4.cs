using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace WhiteCat.Tween
{
	/// <summary>
	/// Vector4 类型的插值动画
	/// </summary>
	public abstract class TweenVector4 : TweenFromTo<Vector4>
	{
		/// <summary> X 通道开关 </summary>
		public bool toggleX = true;

		/// <summary> Y 通道开关 </summary>
		public bool toggleY = true;

		/// <summary> Z 通道开关 </summary>
		public bool toggleZ = true;

		/// <summary> W 通道开关 </summary>
		public bool toggleW = true;


		// 根据插值系数更改插值状态
		public override void OnTween(float factor)
		{
			var temp = current;
			if (toggleX) temp.x = (_to.x - _from.x) * factor + _from.x;
			if (toggleY) temp.y = (_to.y - _from.y) * factor + _from.y;
			if (toggleZ) temp.z = (_to.z - _from.z) * factor + _from.z;
			if (toggleW) temp.w = (_to.w - _from.w) * factor + _from.w;
			current = temp;
		}

#if UNITY_EDITOR

		SerializedProperty[] _toggleProperties;
		SerializedProperty[] _fromProperties;
		SerializedProperty[] _toProperties;


		public static SerializedProperty[] GetVector4Properties(SerializedProperty vector4Property)
		{
			SerializedProperty[] properties = new SerializedProperty[4];
			properties[0] = vector4Property.FindPropertyRelative("x");
			properties[1] = vector4Property.FindPropertyRelative("y");
			properties[2] = vector4Property.FindPropertyRelative("z");
			properties[3] = vector4Property.FindPropertyRelative("w");
			return properties;
		}


		protected override void Editor_OnEnable()
		{
			base.Editor_OnEnable();

			_toggleProperties = new SerializedProperty[4];
			_toggleProperties[0] = editor.serializedObject.FindProperty("toggleX");
			_toggleProperties[1] = editor.serializedObject.FindProperty("toggleY");
			_toggleProperties[2] = editor.serializedObject.FindProperty("toggleZ");
			_toggleProperties[3] = editor.serializedObject.FindProperty("toggleW");

			_fromProperties = GetVector4Properties(_fromProperty);
			_toProperties = GetVector4Properties(_toProperty);
		}


		protected override void Editor_OnDisable()
		{
			base.Editor_OnDisable();
			_toggleProperties = null;
			_fromProperties = null;
			_toProperties = null;
		}


		protected override void DrawExtraFields()
		{
			DrawFromToChannels();
		}


		protected void DrawFromToChannels()
		{
			FloatChannelField(_toggleProperties[0], "X", _fromProperties[0], _toProperties[0]);
			FloatChannelField(_toggleProperties[1], "Y", _fromProperties[1], _toProperties[1]);
			FloatChannelField(_toggleProperties[2], "Z", _fromProperties[2], _toProperties[2]);
			FloatChannelField(_toggleProperties[3], "W", _fromProperties[3], _toProperties[3]);
		}


		protected void DrawClampedFromToChannels(float min, float max)
		{
			ClampedFloatChannelField(_toggleProperties[0], "X", _fromProperties[0], _toProperties[0], min, max);
			ClampedFloatChannelField(_toggleProperties[1], "Y", _fromProperties[1], _toProperties[1], min, max);
			ClampedFloatChannelField(_toggleProperties[2], "Z", _fromProperties[2], _toProperties[2], min, max);
			ClampedFloatChannelField(_toggleProperties[3], "W", _fromProperties[3], _toProperties[3], min, max);
		}

#endif // UNITY_EDITOR

	} // class TweenVector4

} // namespace WhiteCat.Tween