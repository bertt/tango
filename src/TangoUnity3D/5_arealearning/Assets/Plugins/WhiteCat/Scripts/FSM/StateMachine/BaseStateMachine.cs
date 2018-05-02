using System;

namespace WhiteCat.FSM
{
	/// <summary>
	/// 状态机基类, 可作为顶层状态机或子状态机使用.
	/// 顶层状态机需要手动调用 OnUpdate 来更新状态
	/// </summary>
	public class BaseStateMachine : BaseState
	{
		IState _currentState;
		float _currentStateTime;


		/// <summary>
		/// 当前状态持续时间
		/// </summary>
		public float currentStateTime
		{
			get { return _currentStateTime; }
		}


		/// <summary>
		/// 当前状态
		/// </summary>
		public IState currentState
		{
			get { return _currentState; }
			set
			{
				if (_currentState != null)
				{
					_currentState.OnExit();
				}

				Kit.Swap(ref value, ref _currentState);
				_currentStateTime = 0;

				if (_currentState != null)
				{
					_currentState.OnEnter();
				}

				OnStateChanged(value, _currentState);
			}
		}


		/// <summary>
		/// 当前状态组件. 用于序列化事件
		/// </summary>
		public BaseState currentStateComponent
		{
			get { return _currentState as BaseState; }
			set { currentState = value; }
		}


		/// <summary>
		/// 状态变化后触发的事件
		/// </summary>
		protected virtual void OnStateChanged(IState prevState, IState currentState)
		{
		}


		/// <summary>
		/// 当此状态机作为子状态机使用时, OnEnter 通知状态激活
		/// </summary>
		public override void OnEnter()
		{
		}


		/// <summary>
		/// 当此状态机作为子状态机使用时, OnExit 通知状态退出
		/// </summary>
		public override void OnExit()
		{
		}


		/// <summary>
		/// 更新当前状态. 子类实现必须调用父类方法
		/// 注意: 仅顶层状态机需要手动调用此方法, 子状态机不应当调用
		/// </summary>
		public override void OnUpdate(float deltaTime)
		{
			_currentStateTime += deltaTime;
			if (_currentState != null)
			{
				_currentState.OnUpdate(deltaTime);
			}
		}


		/// <summary>
		/// 更新当前状态
		/// 注意: 仅顶层状态机需要手动调用此方法, 子状态机不应当调用
		/// </summary>
		[Obsolete("Please use OnUpdate instead.")]
		protected void UpdateCurrentState(float deltaTime)
		{
			OnUpdate(deltaTime);
		}

	} // class BaseStateMachine

} // namespace WhiteCat.FSM