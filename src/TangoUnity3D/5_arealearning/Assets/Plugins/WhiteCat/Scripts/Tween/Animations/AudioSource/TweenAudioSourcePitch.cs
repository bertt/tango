using UnityEngine;

namespace WhiteCat.Tween
{
	/// <summary>
	/// 音调插值动画
	/// </summary>
	[AddComponentMenu("White Cat/Tween/Audio Source/Tween Audio Source Pitch")]
	[RequireComponent(typeof(AudioSource))]
	public class TweenAudioSourcePitch : TweenFloat
	{
		AudioSource _audioSource;
		public AudioSource audioSource
		{
			get
			{
				if (!_audioSource)
				{
					_audioSource = GetComponent<AudioSource>();
				}
				return _audioSource;
			}
		}


		public override float from
		{
			get { return _from; }
			set { _from = Mathf.Clamp(value, -3f, 3f); }
		}


		public override float to
		{
			get { return _to; }
			set { _to = Mathf.Clamp(value, -3f, 3f); }
		}


		public override float current
		{
			get { return audioSource.pitch; }
			set { audioSource.pitch = value; }
		}

#if UNITY_EDITOR

		protected override void DrawExtraFields()
		{
			DrawClampedFromToValuesWithSlider(-3f, 3f);
		}

#endif // UNITY_EDITOR

	} // class TweenAudioSourcePitch

} // namespace WhiteCat.Tween