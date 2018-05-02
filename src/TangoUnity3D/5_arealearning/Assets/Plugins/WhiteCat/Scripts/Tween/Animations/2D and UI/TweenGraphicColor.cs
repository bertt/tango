using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace WhiteCat.Tween
{
	/// <summary>
	/// UI Graphic 颜色插值动画
	/// </summary>
	[AddComponentMenu("White Cat/Tween/2D and UI/Tween Graphic Color")]
	public class TweenGraphicColor : TweenColor
	{
		Graphic _graphic;
		public Graphic graphic
		{
			get
			{
				if (!_graphic)
				{
					_graphic = GetComponent<Graphic>();
				}
				return _graphic;
			}
		}


		public override Color current
		{
			get
			{
				if(graphic) return graphic.color;
				else return Color.white;
			}
			set
			{
				if (graphic) graphic.color = value;
			}
		}

#if UNITY_EDITOR

		protected override void DrawExtraFields()
		{
			if (!graphic)
			{
				EditorGUILayout.HelpBox("Require UI graphic component.", MessageType.Error);
				EditorGUILayout.Space();
			}
			base.DrawExtraFields();
		}

#endif // UNITY_EDITOR

	} // class TweenGraphicColor

} // namespace WhiteCat.Tween