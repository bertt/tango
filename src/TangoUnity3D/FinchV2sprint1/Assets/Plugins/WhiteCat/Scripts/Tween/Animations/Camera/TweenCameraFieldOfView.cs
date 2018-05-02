using UnityEngine;

namespace WhiteCat.Tween
{
	/// <summary>
	/// FOV 动画
	/// </summary>
	[AddComponentMenu("White Cat/Tween/Camera/Tween Camera Field of View")]
	[RequireComponent(typeof(Camera))]
	public class TweenCameraFieldOfView : TweenFloat
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


		public override float from
		{
			get { return _from; }
			set { _from = Mathf.Clamp(value, 1f, 179f); }
		}


		public override float to
		{
			get { return _to; }
			set { _to = Mathf.Clamp(value, 1f, 179f); }
		}


		public override float current
		{
			get { return targetCamera.fieldOfView; }
			set { targetCamera.fieldOfView = value; }
		}

#if UNITY_EDITOR

		protected override void DrawExtraFields()
		{
			DrawClampedFromToValuesWithSlider(1f, 179f);
		}

#endif // UNITY_EDITOR

	} // class TweenCameraFieldOfView

} // namespace WhiteCat.Tween