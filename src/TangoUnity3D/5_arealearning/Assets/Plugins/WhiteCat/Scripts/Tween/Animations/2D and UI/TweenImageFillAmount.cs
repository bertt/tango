using UnityEngine;
using UnityEngine.UI;

namespace WhiteCat.Tween
{
	/// <summary>
	/// Image fillAmount 插值动画
	/// </summary>
	[AddComponentMenu("White Cat/Tween/2D and UI/Tween Image Fill Amount")]
	[RequireComponent(typeof(Image))]
	public class TweenImageFillAmount : TweenFloat
	{
		Image _image;
		public Image image
		{
			get
			{
				if (!_image)
				{
					_image = GetComponent<Image>();
				}
				return _image;
			}
		}


		public override float from
		{
			get { return _from; }
			set { _from = Mathf.Clamp01(value); }
		}


		public override float to
		{
			get { return _to; }
			set { _to = Mathf.Clamp01(value); }
		}


		public override float current
		{
			get { return image.fillAmount; }
			set { image.fillAmount = value; }
		}

#if UNITY_EDITOR

		protected override void DrawExtraFields()
		{
			DrawClampedFromToValuesWithSlider(0f, 1f);
		}

#endif // UNITY_EDITOR

	} // class TweenImageFillAmount

} // namespace WhiteCat.Tween