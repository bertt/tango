using System;
using UnityEngine;
using UnityEngine.Events;

namespace WhiteCat.FSM
{
	/// <summary>
	/// 通用状态机组件
	/// </summary>
	[AddComponentMenu("White Cat/FSM/State Machine")]
	public class StateMachine : BaseStateMachine
	{
		/// <summary>
		/// 状态机更新模式
		/// </summary>
		public UpdateMode updateMode = UpdateMode.Update;


		/// <summary>
		/// 初始状态
		/// </summary>
		public BaseState startState;


		[Header("Sub-state Machine Events")]
		[SerializeField]
		UnityEvent _onEnter;

		[SerializeField]
		UnityEvent _onExit;


		// 是否作为子状态机使用
		bool _isSubStateMachine = false;


		// 设置初始状态
		void Start()
		{
			if (startState)
			{
				currentStateComponent = startState;
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


		// 当作为子状态机使用时, 需要停止主动调用 OnUpdate
		public override void OnEnter()
		{
			_isSubStateMachine = true;

			if (_onEnter != null)
			{
				_onEnter.Invoke();
			}
		}


		// 当不再作为子状态机使用时, 才允许主动调用 OnUpdate
		public override void OnExit()
		{
			_isSubStateMachine = false;

			if (_onExit != null)
			{
				_onExit.Invoke();
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

	} // class StateMachine

} // namespace WhiteCat.FSM