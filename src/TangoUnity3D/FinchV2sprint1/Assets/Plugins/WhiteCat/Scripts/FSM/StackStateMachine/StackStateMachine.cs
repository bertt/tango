using System;
using UnityEngine;
using UnityEngine.Events;

namespace WhiteCat.FSM
{
	/// <summary>
	/// 通用栈状态机组件
	/// </summary>
	[AddComponentMenu("White Cat/FSM/Stack State Machine")]
	public class StackStateMachine : BaseStackStateMachine
	{
		/// <summary>
		/// 状态机更新模式
		/// </summary>
		public UpdateMode updateMode = UpdateMode.Update;


		/// <summary>
		/// 初始状态
		/// </summary>
		public BaseStackState startState;


		[Header("Sub-state Machine Events")]
		[SerializeField]
		UnityEvent _onEnter;

		[SerializeField]
		UnityEvent _onExit;

		[SerializeField]
		UnityEvent _onPush;

		[SerializeField]
		UnityEvent _onPop;


		// 是否作为子状态机使用
		bool _isSubStateMachine = false;


		// 设置初始状态
		void Start()
		{
			if (startState)
			{
				PushStateComponent(startState);
			}
		}


		// Update 更新状态
		void Update()
		{
			if (!_isSubStateMachine && updateMode == UpdateMode.Update)
			{
				OnUpdate(Time.deltaTime);
			}
		}


		// LateUpdate 更新状态
		void LateUpdate()
		{
			if (!_isSubStateMachine && updateMode == UpdateMode.LateUpdate)
			{
				OnUpdate(Time.deltaTime);
			}
		}


		// FixedUpdate 更新状态
		void FixedUpdate()
		{
			if (!_isSubStateMachine && updateMode == UpdateMode.FixedUpdate)
			{
				OnUpdate(Time.deltaTime);
			}
		}


		/// <summary>
		/// 添加或移除更新状态触发的事件
		/// </summary>
		public event Action<float> onUpdate;


		/// <summary>
		/// 添加或移除进入状态触发的事件 (作为子状态机)
		/// </summary>
		public event UnityAction onEnter
		{
			add
			{
				if (_onEnter == null)
				{
					_onEnter = new UnityEvent();
				}
				_onEnter.AddListener(value);
			}
			remove
			{
				if (_onEnter != null)
				{
					_onEnter.RemoveListener(value);
				}
			}
		}


		/// <summary>
		/// 添加或移除离开状态触发的事件 (作为子状态机)
		/// </summary>
		public event UnityAction onExit
		{
			add
			{
				if (_onExit == null)
				{
					_onExit = new UnityEvent();
				}
				_onExit.AddListener(value);
			}
			remove
			{
				if (_onExit != null)
				{
					_onExit.RemoveListener(value);
				}
			}
		}


		/// <summary>
		/// 添加或移除状态入栈触发的事件
		/// </summary>
		public event UnityAction onPush
		{
			add
			{
				if (_onPush == null)
				{
					_onPush = new UnityEvent();
				}
				_onPush.AddListener(value);
			}
			remove
			{
				if (_onPush != null)
				{
					_onPush.RemoveListener(value);
				}
			}
		}


		/// <summary>
		/// 添加或移除状态出栈触发的事件
		/// </summary>
		public event UnityAction onPop
		{
			add
			{
				if (_onPop == null)
				{
					_onPop = new UnityEvent();
				}
				_onPop.AddListener(value);
			}
			remove
			{
				if (_onPop != null)
				{
					_onPop.RemoveListener(value);
				}
			}
		}


		public override void OnEnter()
		{
			if (_onEnter != null)
			{
				_onEnter.Invoke();
			}
		}


		public override void OnExit()
		{
			if (_onExit != null)
			{
				_onExit.Invoke();
			}
		}


		// 当作为子状态机使用时, 需要停止主动调用 OnUpdate
		public override void OnPush()
		{
			_isSubStateMachine = true;

			if (_onPush != null)
			{
				_onPush.Invoke();
			}
		}


		// 当不再作为子状态机使用时, 才允许主动调用 OnUpdate
		public override void OnPop()
		{
			_isSubStateMachine = false;

			if (_onPop != null)
			{
				_onPop.Invoke();
			}
		}


		public override void OnUpdate(float deltaTime)
		{
			base.OnUpdate(deltaTime);

			if (onUpdate != null)
			{
				onUpdate(deltaTime);
			}
		}

	} // class StackStateMachine

} // namespace WhiteCat.FSM