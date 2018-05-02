using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace WhiteCat.Tween
{
	/// <summary>
	/// 插值器组件
	/// </summary>
	[AddComponentMenu("White Cat/Tween/Tweener")]
	[ExecuteInEditMode]
	public partial class Tweener : ScriptableComponentWithEditor
	{
		#region Fields

		const float _minDuration = 0.001f;      // 最小持续时间

		[SerializeField] Interpolator _interpolator = new Interpolator();

		[SerializeField] float _startDelay = 0f;
		[SerializeField] [Min(_minDuration)] float _duration = 1f;

		[SerializeField] WrapMode _wrapMode = WrapMode.PingPong;
		[SerializeField] PlayMode _playMode = PlayMode.BothStop;

		[SerializeField] UpdateMode _updateMode = UpdateMode.Update;
		[SerializeField] TimeMode _timeMode = TimeMode.Normal;

		[SerializeField] UnityEvent _onForwardToEnding = new UnityEvent();
		[SerializeField] UnityEvent _onBackToBeginning = new UnityEvent();

		bool _isForward = true;
		float _startCountdown = 0f;
		float _normalizedTime = 0f;

		List<TweenAnimation> _animations = new List<TweenAnimation>(4);

		#endregion // Fields


		#region Properties

		/// <summary>
		/// 延迟播放时间
		/// </summary>
		public float startDelay
		{
			get { return _startDelay; }
			set { _startDelay = value; }
		}


		/// <summary>
		/// 持续时间
		/// </summary>
		public float duration
		{
			get { return _duration; }
			set { _duration = value > _minDuration ? value : _minDuration; }
		}


		/// <summary>
		/// 循环模式
		/// </summary>
		public WrapMode wrapMode
		{
			get { return _wrapMode; }
			set { _wrapMode = value; }
		}


		/// <summary>
		/// 播放模式
		/// </summary>
		public PlayMode playMode
		{
			get { return _playMode; }
			set { _playMode = value; }
		}


		/// <summary>
		/// 更新模式. 当更新模式为 FixedUpdate 时, timeMode 被忽略
		/// </summary>
		public UpdateMode updateMode
		{
			get { return _updateMode; }
			set { _updateMode = value; }
		}


		/// <summary>
		/// 时间模式
		/// </summary>
		public TimeMode timeMode
		{
			get { return _timeMode; }
			set { _timeMode = value; }
		}


		/// <summary>
		/// 前进到终点的事件
		/// </summary>
		public event UnityAction onForwardToEnding
		{
			add { _onForwardToEnding.AddListener(value); }
			remove { _onForwardToEnding.RemoveListener(value); }
		}


		/// <summary>
		/// 后退到起点的事件
		/// </summary>
		public event UnityAction onBackToBeginning
		{
			add { _onBackToBeginning.AddListener(value); }
			remove { _onBackToBeginning.RemoveListener(value); }
		}


		/// <summary>
		/// 播放方向
		/// </summary>
		public bool isForward
		{
			get { return _isForward; }
			set { _isForward = value; }
		}


		/// <summary>
		/// 单位化的时间
		/// </summary>
		public float normalizedTime
		{
			get { return _normalizedTime; }
			set
			{
				_normalizedTime = Mathf.Clamp01(value);
				float factor = _interpolator.Evaluate(_normalizedTime);

				for (int i = 0; i < _animations.Count; i++)
				{
					var item = _animations[i];
					if (item) item.OnTween(factor);
					else _animations.RemoveAt(i--);
				}
			}
		}

		#endregion // Properties


		#region Methods

		/// <summary>
		/// 记录所有关联动画的状态
		/// </summary>
		public void RecordAnimationStates()
		{
			for (int i = 0; i < _animations.Count; i++)
			{
				if (_animations[i])
				{
					_animations[i].OnRecord();
				}
				else
				{
					_animations.RemoveAt(i--);
				}
			}
		}


		/// <summary>
		/// 恢复所有关联动画的状态
		/// </summary>
		public void RestoreAnimationStates()
		{
			for (int i = 0; i < _animations.Count; i++)
			{
				if (_animations[i])
				{
					_animations[i].OnRestore();
				}
				else
				{
					_animations.RemoveAt(i--);
				}
			}
		}


		/// <summary>
		/// 颠倒播放方向
		/// </summary>
		public void ReverseDirection()
		{
			_isForward = !_isForward;
		}


		// 更新插值动画
		protected void UpdateTween(float deltaTime)
		{
			while(enabled)
			{
				// 延迟
				if (_startCountdown > 0f)
				{
					_startCountdown -= deltaTime;

					if (_startCountdown > 0f)
					{
						return;
					}

					deltaTime = -_startCountdown;
					_startCountdown = 0f;
				}

				// 判断是否消耗完所有时间 (或暂停)
				if (deltaTime <= Mathf.Epsilon)
				{
					normalizedTime = _normalizedTime;
					return;
				}

				if (_isForward)
				{
					if (wrapMode == WrapMode.Clamp && _normalizedTime == 1f)
					{
						normalizedTime = 1f;
						return;
					}

					float time = _normalizedTime * _duration + deltaTime;

					if (time < _duration)
					{
						normalizedTime = time / _duration;
						return;
					}

					deltaTime = time - _duration;

					// 处理 Wrap Mode
					if (wrapMode == WrapMode.Loop)
					{
						normalizedTime = 0f;
					}
					else
					{
						normalizedTime = 1f;

						if (wrapMode == WrapMode.PingPong)
						{
							_isForward = false;
						}
					}

					// 处理 Play Mode 和触发事件
					if ((playMode & PlayMode.StopWhenForwardToEnding) != 0)
					{
						enabled = false;
					}

					_onForwardToEnding.Invoke();

					if(wrapMode == WrapMode.Clamp)
					{
						return;
					}
				}
				else
				{
					if (wrapMode == WrapMode.Clamp && _normalizedTime == 0f)
					{
						normalizedTime = 0f;
						return;
					}

					float time = _normalizedTime * _duration - deltaTime;

					if (time > 0f)
					{
						normalizedTime = time / _duration;
						return;
					}

					deltaTime = -time;

					// 处理 Wrap Mode
					if (wrapMode == WrapMode.Loop)
					{
						normalizedTime = 1f;
					}
					else
					{
						normalizedTime = 0f;

						if (wrapMode == WrapMode.PingPong)
						{
							_isForward = true;
						}
					}

					// 处理 Play Mode 和触发事件
					if ((playMode & PlayMode.StopWhenBackToBeginning) != 0)
					{
						enabled = false;
					}

					_onBackToBeginning.Invoke();

					if (wrapMode == WrapMode.Clamp)
					{
						return;
					}
				}
			}
		}


		// 开始播放时进行延时倒计时
		void OnEnable()
		{
			_startCountdown = startDelay;
		}


		// Update 每帧更新状态
		void Update()
		{
			if (_updateMode == UpdateMode.Update)
			{
#if UNITY_EDITOR
				if (Application.isPlaying && !_isDraggingInEditor)
#endif
					UpdateTween(timeMode == TimeMode.Normal ? Time.deltaTime : Time.unscaledDeltaTime);
			}
		}


		// LateUpdate 每帧更新状态
		void LateUpdate()
		{
			if (_updateMode == UpdateMode.LateUpdate)
			{
#if UNITY_EDITOR
				if (Application.isPlaying && !_isDraggingInEditor)
#endif
					UpdateTween(timeMode == TimeMode.Normal ? Time.deltaTime : Time.unscaledDeltaTime);
			}
		}


		// FixedUpdate 每帧更新状态
		void FixedUpdate()
		{
			if (_updateMode == UpdateMode.FixedUpdate)
			{
#if UNITY_EDITOR
				if (Application.isPlaying && !_isDraggingInEditor)
#endif
					UpdateTween(Time.fixedDeltaTime);
			}
		}

		#endregion // Methods

	} // class Tweener

} // namespace WhiteCat.Tween