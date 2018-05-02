
namespace WhiteCat.FSM
{
	/// <summary>
	/// 栈状态组件基类
	/// </summary>
	public abstract class BaseStackState : BaseState, IStackState
	{
		public abstract void OnPop();
		public abstract void OnPush();
	}

} // namespace WhiteCat.FSM