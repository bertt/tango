using UnityEngine;

namespace WhiteCat.Tween
{
	/// <summary>
	/// 相机背景色插值动画
	/// </summary>
	[AddComponentMenu("White Cat/Tween/Camera/Tween Camera Background Color")]
	[RequireComponent(typeof(Camera))]
	public class TweenCameraBackgroundColor : TweenColor
	{
		Camera _camera;
		public Camera targetCamera
		{
			get
			{
				if (!_camera)
				{
					_camera = GetComponent<Camera>();
				}
				return _camera;
			}
		}


		public override Color current
		{
			get { return targetCamera.backgroundColor; }
			set { targetCamera.backgroundColor = value; }
		}

	} // class TweenCameraBackgroundColor

} // namespace WhiteCat.Tween