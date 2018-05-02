using System;
using UnityEngine;
using UnityEngine.Events;

namespace WhiteCat.FSM
{
	/// <summary>
	/// 可序列化栈状态. 状态的 Enter, Exit, Push 和 Pop 事件可序列化
	/// </summary>
	[Serializable]
	public class SerializableStackState : SerializableState, IStackState
    {
		[SerializeField]
		UnityEvent _onPush;

		[SerializeField]
		UnityEvent _onPop;


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


		void IStackState.OnPush()
		{
			if (_onPush != null)
			{
				_onPush.Invoke();
			}
		}


		void IStackState.OnPop()
		{
			if (_onPop != null)
			{
				_onPop.Invoke();
			}
		}

	} // class SerializableStackState

} // namespace WhiteCat.FSM