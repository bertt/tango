using System;
using UnityEngine;
using UnityEngine.Events;

namespace WhiteCat.FSM
{
	/// <summary>
	/// 通用栈状态组件. 状态的 Enter, Exit, Push 和 Pop 事件可序列化
	/// </summary>
	[AddComponentMenu("White Cat/FSM/Stack State")]
    public class StackState : BaseStackState
	{
		[SerializeField]
		UnityEvent _onEnter;

		[SerializeField]
		UnityEvent _onExit;

		[SerializeField]
		UnityEvent _onPush;

		[SerializeField]
		UnityEvent _onPop;


		/// <summary>
		/// 添加或移除更新状态触发的事件
		/// </summary>
		public event Action<float> onUpdate;


		/// <summary>
		/// 添加或移除进入状态触发的事件
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
		/// 添加或移除离开状态触发的事件
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


		public override void OnPush()
		{
			if (_onPush != null)
			{
				_onPush.Invoke();
			}
		}


		public override void OnPop()
		{
			if (_onPop != null)
			{
				_onPop.Invoke();
			}
		}


		public override void OnUpdate(float deltaTime)
		{
			if (onUpdate != null)
			{
				onUpdate(deltaTime);
			}
		}

	} // class StackState

} // namespace WhiteCat.FSM