using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using WhiteCatEditor;
#endif

namespace WhiteCat.Tween
{
	/// <summary>
	/// Quaternion 类型的插值动画
	/// </summary>
	public abstract partial class TweenQuaternion : TweenFromTo<Quaternion>
	{
		// 根据插值系数更改插值状态
		public override void OnTween(float factor)
		{
			current = Quaternion.SlerpUnclamped(_from, _to, factor);
		}

#if UNITY_EDITOR
		
		Quaternion _formQuaternion = Quaternion.identity;
		Quaternion _toQuaternion = Quaternion.identity;

		Vector3 _fromAngle;
		Vector3 _toAngle;


		protected override void DrawExtraFields()
		{
			DrawFromToAngles();
		}


		protected void DrawFromToAngles()
		{
			if (_formQuaternion != _fromProperty.quaternionValue)
			{
				_formQuaternion = _fromProperty.quaternionValue;
				_fromAngle = _formQuaternion.eulerAngles;
			}

			if (_toQuaternion != _toProperty.quaternionValue)
			{
				_toQuaternion = _toProperty.quaternionValue;
				_toAngle = _toQuaternion.eulerAngles;
			}

			EditorKit.RecordAndSetLabelWidth(EditorGUIUtility.currentViewWidth * 0.2f);
			EditorKit.RecordAndSetWideMode(true);

			EditorGUI.BeginChangeCheck();
			_fromAngle = EditorGUILayout.Vector3Field("From", _fromAngle);
			if (EditorGUI.EndChangeCheck())
			{
				_fromProperty.quaternionValue = _formQuaternion = Quaternion.Euler(_fromAngle);
			}

			EditorGUI.BeginChangeCheck();
			_toAngle = EditorGUILayout.Vector3Field("To", _toAngle);
			if (EditorGUI.EndChangeCheck())
			{
				_toProperty.quaternionValue = _toQuaternion = Quaternion.Euler(_toAngle);
			}

			EditorKit.RestoreLabelWidth();
			EditorKit.RestoreWideMode();
		}

#endif // UNITY_EDITOR

	} // class TweenQuaternion

} // namespace WhiteCat.Tween