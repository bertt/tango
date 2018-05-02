using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace WhiteCat.Tween
{
	/// <summary>
	/// Vector2 类型的插值动画
	/// </summary>
	public abstract class TweenVector2 : TweenFromTo<Vector2>
	{
		/// <summary> X 通道开关 </summary>
		public bool toggleX = true;

		/// <summary> Y 通道开关 </summary>
		public bool toggleY = true;


		// 根据插值系数更改插值状态
		public override void OnTween(float factor)
		{
			var temp = current;
			if (toggleX) temp.x = (_to.x - _from.x) * factor + _from.x;
			if (toggleY) temp.y = (_to.y - _from.y) * factor + _from.y;
			current = temp;
		}

#if UNITY_EDITOR

		SerializedProperty[] _toggleProperties;
		SerializedProperty[] _fromProperties;
		SerializedProperty[] _toProperties;


		protected static SerializedProperty[] GetVector2Properties(SerializedProperty vector2Property)
		{
			SerializedProperty[] properties = new SerializedProperty[2];
			properties[0] = vector2Property.FindPropertyRelative("x");
			properties[1] = vector2Property.FindPropertyRelative("y");
			return properties;
		}


		protected override void Editor_OnEnable()
		{
			base.Editor_OnEnable();

			_toggleProperties = new SerializedProperty[2];
			_toggleProperties[0] = editor.serializedObject.FindProperty("toggleX");
			_toggleProperties[1] = editor.serializedObject.FindProperty("toggleY");

			_fromProperties = GetVector2Properties(_fromProperty);
			_toProperties = GetVector2Properties(_toProperty);
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
		}


		protected void DrawClampedFromToChannels(float min, float max)
		{
			ClampedFloatChannelField(_toggleProperties[0], "X", _fromProperties[0], _toProperties[0], min, max);
			ClampedFloatChannelField(_toggleProperties[1], "Y", _fromProperties[1], _toProperties[1], min, max);
		}

#endif // UNITY_EDITOR

	} // class TweenVector2

} // namespace WhiteCat.Tween