using UnityEngine;

namespace WhiteCat.Tween
{
	/// <summary>
	/// Sprite Renderer 颜色插值动画
	/// </summary>
	[AddComponentMenu("White Cat/Tween/2D and UI/Tween Sprite Renderer Color")]
	[RequireComponent(typeof(SpriteRenderer))]
	public class TweenSpriteRendererColor : TweenColor
	{
		SpriteRenderer _spriteRenderer;
		public SpriteRenderer spriteRenderer
		{
			get
			{
				if (!_spriteRenderer)
				{

					_spriteRenderer = GetComponent<SpriteRenderer>();
				}
				return _spriteRenderer;
			}
		}


		public override Color current
		{
			get { return spriteRenderer.color; }
			set { spriteRenderer.color = value; }
		}

	} // class TweenSpriteRendererColor

} // namespace WhiteCat.Tween