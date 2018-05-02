using UnityEngine;

namespace WhiteCat.Tween
{
	/// <summary>
	/// 音量插值动画
	/// </summary>
	[AddComponentMenu("White Cat/Tween/Audio Source/Tween Audio Source Volume")]
	[RequireComponent(typeof(AudioSource))]
	public class TweenAudioSourceVolume : TweenFloat
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
			set { _from = Mathf.Clamp01(value); }
		}


		public override float to
		{
			get { return _to; }
			set { _to = Mathf.Clamp01(value); }
		}


		public override float current
		{
			get { return audioSource.volume; }
			set { audioSource.volume = value; }
		}

#if UNITY_EDITOR

		protected override void DrawExtraFields()
		{
			DrawClampedFromToValuesWithSlider(0f, 1f);
		}

#endif // UNITY_EDITOR

	} // class TweenAudioSourceVolume

} // namespace WhiteCat.Tween