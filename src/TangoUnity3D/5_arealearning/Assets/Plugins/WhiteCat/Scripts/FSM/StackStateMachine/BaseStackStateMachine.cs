using System;
using System.Collections.Generic;

namespace WhiteCat.FSM
{
	/// <summary>
	/// 栈状态机基类, 可作为顶层状态机或子状态机使用.
	/// 顶层状态机需要手动调用 OnUpdate 来更新状态
	/// </summary>
	public class BaseStackStateMachine : BaseStackState
	{
		IStackState _currentState;
		float _currentStateTime;
		Stack<IStackState> _states = new Stack<IStackState>(4);


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
		public IStackState currentState
		{
			get { return _currentState; }
		}


		/// <summary>
		/// 栈中状态的总数. 包括压入的空状态
		/// </summary>
		public int stateCount
		{
			get { return _states.Count; }
		}


		/// <summary>
		/// 将新状态压入栈
		/// </summary>
		public void PushState(IStackState newState)
		{
			if (_currentState != null)
			{
				_currentState.OnExit();
			}

			Kit.Swap(ref _currentState, ref newState);
			_currentStateTime = 0;
			_states.Push(_currentState);

			if (_currentState != null)
			{
				_currentState.OnPush();
                _currentState.OnEnter();
			}

			OnStateChanged(newState, _currentState);
		}


		/// <summary>
		/// 将新状态压入栈. 用于序列化事件
		/// </summary>
		public void PushStateComponent(BaseStackState newState)
		{
			PushState(newState);
        }


		/// <summary>
		/// 将当前状态弹出栈
		/// </summary>
		public void PopState()
		{
			PopStates(1);
		}


		/// <summary>
		/// 连续弹出多个状态
		/// </summary>
		public void PopStates(int count)
		{
			if (count > _states.Count) count = _states.Count;
			if (count <= 0) return;

			if (_currentState != null)
			{
				_currentState.OnExit();
			}

			IStackState state;
			while(count > 0)
			{
				state = _states.Pop();

				if (state != null) state.OnPop();

				count--;
			}

			state = _currentState;
			_currentState = (_states.Count > 0) ? _states.Peek() : null;
			_currentStateTime = 0f;

			if (_currentState != null)
			{
				_currentState.OnEnter();
			}

			OnStateChanged(state, _currentState);
		}


		/// <summary>
		/// 状态变化后触发的事件
		/// </summary>
		protected virtual void OnStateChanged(IStackState prevState, IStackState currentState)
		{
		}


		/// <summary>
		/// 当此状态机作为子状态机使用时, OnPop 通知状态出栈
		/// </summary>
		public override void OnPop()
		{
		}


		/// <summary>
		/// 当此状态机作为子状态机使用时, OnPush 通知状态入栈
		/// </summary>
		public override void OnPush()
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

	} // class BaseStackStateMachine

} // namespace WhiteCat.FSM